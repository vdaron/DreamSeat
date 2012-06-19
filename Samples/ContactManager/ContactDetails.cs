using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DreamSeat;
using MindTouch.Tasking;
using System.IO;
using Newtonsoft.Json.Linq;

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
				if (theContact == null)
					this.theDeleteButton.Enabled = false;
				else
					this.theDeleteButton.Enabled = true;
			}
		}

		private void RefreshContact()
		{
			if (theContact != null)
			{
				theFirstNameTextBox.Text = theContact.FirstName;
				theLastNameTextBox.Text = theContact.LastName;
				theCreationDateLabel.Text = theContact.CreationDate.ToString();
				theLastUpdateDateLabel.Text = theContact.LastUpdateDate.ToString();
				StringBuilder emails = new StringBuilder();
				foreach (var email in theContact.EmailAddresses)
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
			if (isNew)
			{
				theContact = new Contact();
			}

			theContact.FirstName = theFirstNameTextBox.Text;
			theContact.LastName = theLastNameTextBox.Text;
			theContact.EmailAddresses.Clear();
			foreach (var email in theEmailsTextBox.Text.Trim().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
			{
				theContact.EmailAddresses.Add(email.Trim());
			}

			if (isNew)
			{
				theContact = Database.CreateDocument<Contact>(theContact, new Result<Contact>()).Wait();
				if (imageAvatar.Image != null && openFileDialog.FileName.Trim() != String.Empty)
				{
					Database.AddAttachment(theContact, openFileDialog.FileName, new Result<JObject>()).Wait();
				}
			}
			else
			{
				try
				{
					theContact = Database.UpdateDocument<Contact>(theContact, new Result<Contact>()).Wait();
					if (imageAvatar.Image != null && openFileDialog.FileName.Trim() != String.Empty)
					{
						Database.AddAttachment(theContact, openFileDialog.FileName, new Result<JObject>()).Wait();
					}

				}
				catch (Exception exc)
				{
					//TODO
				}
			}
		}

		private void theDeleteButton_Click(object sender, EventArgs e)
		{
			bool isNotSelected = theContact == null;
			if (isNotSelected)
			{
				return;
			}
			try
			{
				Database.DeleteDocument(theContact, new Result<JObject>()).Wait();
			}
			catch (Exception exc)
			{
				Console.WriteLine("### Error, please go it again ###\n" + exc);
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
