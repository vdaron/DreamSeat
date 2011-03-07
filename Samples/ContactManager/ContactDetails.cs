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
using System.IO;

namespace ContactManager
{
	
	//public delegate void ErrorUpdatedContactDelegate(object aSender, Contact aContact);


	public partial class ContactDetails : UserControl
	{
		private Contact theContact;
		private CouchDatabase theDatabase;
		
		//public event ErrorUpdatedContactDelegate ErrorUpdatedContact;

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
				theDeleteButton.Enabled = true;
			}
		}

		public Contact CurrentContact
		{
			get { return theContact; }
			set
			{
				theContact = value;
				RefreshContact();
				if(theContact==null)
					this.theDeleteButton.Enabled = false;
				else
					this.theDeleteButton.Enabled = true;
			}
		}

		private void RefreshContact()
		{
			if(theContact != null)
			{
				theFirstNameTextBox.Text = theContact.FirstName;
				theLastNameTextBox.Text = theContact.LastName;
				theCreationDateLabel.Text = theContact.CreationDate.ToString();
				theLastUpdateDateLabel.Text = theContact.LastUpdateDate.ToString();
				StringBuilder emails = new StringBuilder();
				foreach(var email in theContact.EmailAddresses)
				{
					emails.AppendLine(email);
				}
				theEmailsTextBox.Text = emails.ToString();

                if (theContact.HasAttachment)
                {
                    this.imageAvatar.Image = Image.FromStream(Database.GetAttachment(theContact, theContact.GetAttachmentNames().First()));
                }
                else
                {
                    this.imageAvatar.Image = null;
                }
			}
			else
			{
				theFirstNameTextBox.Text = String.Empty;
				theLastNameTextBox.Text = String.Empty;
				theEmailsTextBox.Text = String.Empty;
				theCreationDateLabel.Text = String.Empty;
				theLastUpdateDateLabel.Text = String.Empty;
                imageAvatar.Image = null;
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
				theContact = Database.CreateDocument<Contact>(theContact, new Result<Contact>()).Wait();
                if (openFileDialog.FileName.Trim() != "")
                {
                    JObject j = Database.AddAttachment(theContact, this.openFileDialog.FileName, new Result<JObject>()).Wait();
                    Console.WriteLine("### Result attachement:" +
                                      j.ToString() + " ###");
                }
			}
			else
			{
				try{
					theContact = Database.UpdateDocument<Contact>(theContact, new Result<Contact>()).Wait();
                    if (openFileDialog.FileName.Trim() != "")
                    {
                        JObject j = Database.AddAttachment(theContact, this.openFileDialog.FileName, new Result<JObject>()).Wait();
                        Console.WriteLine("### Result attachement:" +
                                          j.ToString() + " ###");
                    }
                    
				}catch(Exception exc){
					Console.WriteLine("### Error, please go it again ###\n"+exc);
					//ErrorUpdatedContact(this,theContact);
				}
			}
		}
		
		private void theDeleteButton_Click(object sender, EventArgs e)
		{
			bool isNotSelected = theContact == null;
			if(isNotSelected)
			{
				return;
			}
			try{
				Database.DeleteDocument(theContact,new Result<JObject>()).Wait();
			}catch(Exception exc){
					Console.WriteLine("### Error, please go it again ###\n"+exc);
					//ErrorUpdatedContact(this,theContact);
			}
		}

        private void theAvatarButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Database.
                using (FileStream stream = File.OpenRead(openFileDialog.FileName))
                {
                    imageAvatar.Image = Image.FromStream(stream);
                }
                
            }
        }
	}
}
