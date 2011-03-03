using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LoveSeat;
using MindTouch.Tasking;

namespace ContactManager
{
	public partial class MainForm : Form
	{
		private CouchClient theClient;
		private CouchDatabase theDatabase;

		public MainForm()
		{
			InitializeComponent();
		}

		private void quitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			theClient = new CouchClient(Settings.Host,Settings.Port,Settings.Username,Settings.Password);
			theClient.GetDatabase(Settings.DatabaseName, new Result<CouchDatabase>()).WhenDone(
				a => BeginInvoke((MethodInvoker) (() => DatabaseLoaded(a))),
				ErrorManagement.ProcessException
				);
		}

		private void DatabaseLoaded(CouchDatabase aDatabase)
		{
			theDatabase = aDatabase;
			theContactsListBox.Database = theDatabase;
			theChangesListBox.Database = theDatabase;
			theContactDetails.Database = theDatabase;
		}

		private static void ManageException(Exception e)
		{
			
		}

		private void theContactsListBox_SelectedContactChanged(object aSender, Contact aContact)
		{
			theContactDetails.CurrentContact = aContact;
		}
		
		private void theChangesListBox_ContactChanged(object aSender, Contact aContact)
		{
			theContactDetails.Change(aContact);
			theContactsListBox.Change(aContact);
		}
		
		private void theChangesListBox_ContactDeleted(object aSender, string id)
		{
			theContactDetails.Delete(id);
			theContactsListBox.Delete(id);
		}

		private void theContactDetails_NewContact(object sender, Contact aContact)
		{
			theContactDetails.CurrentContact = aContact;
		}
		
		private void theContactDetails_UpdatedContact(object sender, Contact aContact)
		{
			theContactDetails.CurrentContact = aContact;
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			theContactDetails.CurrentContact = null;
		}
	}
}
