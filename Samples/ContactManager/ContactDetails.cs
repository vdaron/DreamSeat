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

namespace ContactManager
{
	public delegate void NewContactDelegate(object sender, Contact aContact);
	public delegate void UpdatedContactDelegate(object sender, Contact aContact);

	public partial class ContactDetails : UserControl
	{
		private Contact theContact;
		private CouchDatabase theDatabase;

		public event NewContactDelegate NewContact;
		public event UpdatedContactDelegate UpdatedContact;

		public ContactDetails()
		{
			InitializeComponent();
		}

		public CouchDatabase Database
		{
			get { return theDatabase; }
			set
			{
				theDatabase = value;
				theSaveButton.Enabled = true;
			}
		}

		public Contact CurrentContact
		{
			get { return theContact; }
			set
			{
				theContact = value;
				RefreshContact();
			}
		}

		private void RefreshContact()
		{
			if(theContact != null)
			{
				theFirstNameTextBox.Text = theContact.FirstName;
				theLastNameTextBox.Text = theContact.LastName;
				StringBuilder emails = new StringBuilder();
				foreach(var email in theContact.EmailAddresses)
				{
					emails.AppendLine(email);
				}
				theEmailsTextBox.Text = emails.ToString();
			}
			else
			{
				theFirstNameTextBox.Text = String.Empty;
				theLastNameTextBox.Text = String.Empty;
				theEmailsTextBox.Text = String.Empty;
			}
		}
		
		public void Change(Contact contactChanged){
			if(theContact!=null && theContact.Equals(contactChanged)){
				Console.WriteLine("### Current contact refreshed ###");
				CurrentContact = contactChanged;
			}
		}

		private void theSaveButton_Click(object sender, EventArgs e)
		{
			bool isNew = theContact == null;
			if(isNew)
			{
				theContact = new Contact();
			}

			theContact.FirstName = theFirstNameTextBox.Text;
			theContact.LastName = theLastNameTextBox.Text;
			theContact.EmailAddresses.Clear();
			foreach (var email in theEmailsTextBox.Text.Trim().Split('\n'))
			{
				theContact.EmailAddresses.Add(email.Trim());
			}

			if(isNew)
			{
				theContact = Database.CreateDocument(theContact, new Result<Contact>()).Wait();
			}
			else
			{
				theContact = Database.UpdateDocument(theContact, new Result<Contact>()).Wait();
			}

			if (isNew && NewContact != null)
				NewContact(this, theContact);
			if(!isNew && UpdatedContact!=null)
				UpdatedContact(this,theContact);
		}
	}
}
