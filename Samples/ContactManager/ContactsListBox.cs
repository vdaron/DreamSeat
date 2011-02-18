using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LoveSeat;
using MindTouch.Tasking;
using Newtonsoft.Json.Linq;

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

		private void DatabaseLoaded()
		{
			if(theDatabase != null)
			{
				ReloadContacts();
			}
		}

		private Yield LoadContactsHelper(Result<ViewResult<object, Contact>> aResult)
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
				                          emit(doc.lastName, doc)
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

			var viewRes = new Result<ViewResult<object, Contact>>();
			yield return theDatabase.GetView("contactview", "all", viewRes);

			if(viewRes.HasException)
			{
				aResult.Throw(viewRes.Exception);
				yield break;
			}
			aResult.Return(viewRes.Value);
		}
	
		private void DisplayContacts(ViewResult<object, Contact> o)
		{
			Items.Clear();
			foreach(var row in o.Rows)
			{
				Items.Add(row.Value);
			}
		}

		private void ContactsListBox_Format(object sender, ListControlConvertEventArgs e)
		{
			Contact c = e.ListItem as Contact;
			if (c == null)
				e.Value = e.ListItem.ToString();
			else
				e.Value = String.Format("{0}, {1}", c.LastName, c.FirstName);
		}

		private void ContactsListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			Contact c = SelectedItem as Contact;
			if ((c != null) && (SelectedContactChanged != null))
				SelectedContactChanged(this, c);
		}

		internal void ReloadContacts()
		{
			Items.Clear();
			Items.Add("Loading Contacts");

			Coroutine.Invoke(LoadContactsHelper, new Result<ViewResult<object, Contact>>()).WhenDone(
				a => BeginInvoke((MethodInvoker)(() => DisplayContacts(a))),
				ErrorManagement.ProcessException
				);
		}
	}
}