using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DreamSeat;
using MindTouch.Tasking;

namespace ContactManager
{
	using Yield = IEnumerator<IYield>;

	public delegate void SelectedContactChangedDelegate(object aSender, Contact aContact);

	public partial class ContactsListBox : ListBox
	{
		private CouchDatabase theDatabase;

		public event SelectedContactChangedDelegate SelectedContactChanged;

		public ContactsListBox()
		{
			InitializeComponent();

			Items.Add("Loading...");
		}

		public CouchDatabase Database
		{
			get { return theDatabase; }
			set
			{
				theDatabase = value;
				DatabaseLoaded();
			}
		}

		public void UpdateContact(Contact aContact)
		{
			for (int i = 0; i < Items.Count; i++)
			{
				if (((Contact)Items[i]).Id == aContact.Id)
				{
					Items.Remove(Items[i]);
					break;
				}
			}

			Items.Add(aContact);
			Sort();
		}

		public void Delete(string id)
		{
			try
			{
				foreach (Contact c in from Contact c in Items where c.Id == id select c)
				{
					Items.Remove(c);
					break;
				}
			}
			catch (Exception)
			{

			}
		}

		private void DatabaseLoaded()
		{
			if(theDatabase != null)
			{
				ReloadContacts();
			}
		}

		private Yield LoadContactsHelper(Result<ViewResult<string,string, Contact>> aResult)
		{
			Result<bool> exists = new Result<bool>();
			yield return theDatabase.DocumentExists("_design/contactview", exists);

			if(exists.HasException){
				aResult.Throw(exists.Exception);
				yield break;
			}

			if (!exists.Value)
			{
				CouchDesignDocument view = new CouchDesignDocument("contactview");
				view.Views.Add("all",
				               new CouchView(
				                  @"function(doc){
				                       if(doc.type && doc.type == 'contact'){
				                          emit(doc.lastName, doc.firstName)
				                       }
				                    }"));

				Result<CouchDesignDocument> creationResult = new Result<CouchDesignDocument>();
				yield return theDatabase.CreateDocument(view, creationResult);

				if(creationResult.HasException)
				{
					aResult.Throw(creationResult.Exception);
					yield break;
				}
			}

			var viewRes = new Result<ViewResult<string, string, Contact>>();
			yield return theDatabase.GetView("contactview", "all",viewRes);

			if(viewRes.HasException)
			{
				aResult.Throw(viewRes.Exception);
				yield break;
			}
			aResult.Return(viewRes.Value);
		}

		private void DisplayContacts(ViewResult<string,string, Contact> o)
		{
			Items.Clear();
			foreach(var row in o.Rows)
			{
				Items.Add(row.Doc);
			}
			Sort();
		}

		private void ContactsListBox_Format(object sender, ListControlConvertEventArgs e)
		{
			Contact c = e.ListItem as Contact;
			e.Value = c == null ? e.ListItem.ToString() : String.Format("{0}, {1}", c.LastName, c.FirstName);
		}

		private void ContactsListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			Contact c = SelectedItem as Contact;
			if ((c != null) && (SelectedContactChanged != null))
				SelectedContactChanged(this, c);
		}

		private void ReloadContacts()
		{
			Items.Clear();
			Items.Add("Loading Contacts");
			
			Coroutine.Invoke(LoadContactsHelper, new Result<ViewResult<string,string, Contact>>()).WhenDone(
				a => BeginInvoke((MethodInvoker)(() => DisplayContacts(a))),
				ErrorManagement.ProcessException
				);
		}
	}
}