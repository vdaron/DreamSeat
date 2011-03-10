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
			theClient = new CouchClient(Settings.Host, Settings.Port, Settings.Username, Settings.Password);

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

		private void theContactsListBox_SelectedContactChanged(object aSender, Contact aContact)
		{
			theContactDetails.CurrentContact = aContact;
		}
		
		private void theChangesListBox_ContactChanged(object aSender, Contact aContact)
		{
			if ((theContactDetails.CurrentContact != null) && (theContactDetails.CurrentContact.Id == aContact.Id))
				theContactDetails.CurrentContact = aContact;
			theContactsListBox.Change(aContact);
		}
		
		private void theChangesListBox_ContactDeleted(object aSender, string id)
		{
			if((theContactDetails.CurrentContact != null)&&(theContactDetails.CurrentContact.Id == id))
				theContactDetails.CurrentContact = null;

			theContactsListBox.Delete(id);
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			theContactDetails.CurrentContact = null;
		}
		
		private void compactToolStripMenuItem_Click(object sender, EventArgs e)
		{
			theDatabase.Compact(new Result()).WhenDone(
				a =>
					{
						if(a.HasException)
							Console.WriteLine("### A problem occured while compacting the data, please contact ###\n" + a.Exception);
						else
							Console.WriteLine("Compact started");
					}
				);
		}

	}
}
