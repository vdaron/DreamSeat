using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DreamSeat;
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

		public Yield LoadContactsHelper(Result<ViewResult<string,string, Contact>> aResult)
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
	
		public void DisplayContacts(ViewResult<string,string, Contact> o)
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
		
		public void Change(Contact contactChanged)
		{
			if(!Items.Contains(contactChanged))
			{
				Items.Add(contactChanged);
				Sort();
			}
			else
			{
				Items.Remove(contactChanged);
				Items.Add(contactChanged);
				Sort();
			}
		}
		
		public void Delete(string id)
		{
			foreach(Contact c in from Contact c in Items where c.Id == id select c)
			{
				Items.Remove(c);
				break;
			}
		}

		internal void ReloadContacts()
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