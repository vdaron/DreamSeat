using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using DreamSeat;
using MindTouch.Tasking;

namespace ContactManager
{
	public delegate void ContactChangedDelegate(object aSender, Contact aContact);

	public delegate void ContactDeletedDelegate(object aSender, string id);

	public partial class ChangesListBox : ListBox
	{
		private CouchDatabase theDatabase;
		public event ContactChangedDelegate ContactChanged;
		public event ContactDeletedDelegate ContactDeleted;
		private CouchContinuousChanges theContinuousChangeManager;

		public ChangesListBox()
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
			if (theDatabase != null)
			{
				Items.Clear();
				ChangeOptions changes = new ChangeOptions();
				changes.Heartbeat = 10000;
				changes.Since = GetSequence();
				theContinuousChangeManager = theDatabase.GetCoutinuousChanges(changes, (x, y) => BeginInvoke((MethodInvoker)(() => OnContactChanged(y))));
			}
		}

		private static void ChangesListBoxFormat(object sender, ListControlConvertEventArgs e)
		{
			CouchChangeResult change = e.ListItem as CouchChangeResult;
			if ((change != null) && (change.Changes.Length >= 1))
			{
				e.Value = String.Format("{0:0000}\t{1}\t{2}", change.Sequence, change.Id, change.Changes[0].ToString());
			}
		}

		private int GetSequence()
		{
			int seqNumber = 1;
			try
			{
				if (File.Exists("sequence.txt"))
				{
					seqNumber = Int32.Parse(File.ReadAllText("sequence.txt"));
					return seqNumber;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("There was a problem while reading the sequence number from sequence.txt\n" + e);
				return seqNumber;
			}
			return seqNumber;
		}
		private void SetSequence(int seq)
		{
			try
			{
				File.WriteAllText("sequence.txt", seq.ToString());
			}
			catch (Exception e)
			{
				Console.WriteLine("There was a problem while writing the sequence number to sequence.txt\n" + e);
			}
		}


		private void OnContactChanged(CouchChangeResult aChangeResult)
		{

			//add to the list of changes
			Items.Add(aChangeResult);

			//record the sequence number of the change so when we run the application we
			//tell that we only need changes > this number. 
			//(Otherwise you get a change message for every doc in the db at the startup)
			SetSequence(aChangeResult.Sequence);

			//verifiy type of change announced so we know if we need to refresh a contact
			if (aChangeResult.Id.StartsWith("_design"))
				return;

			if (aChangeResult.Deleted)
			{
				OnContactDeleted(aChangeResult.Id);
			}
			else
			{
				Console.WriteLine("Retrieving last version of {0}",aChangeResult.Id);
				//get the latest version of this document
				theDatabase.GetDocument(aChangeResult.Id, new Result<Contact>()).WhenDone(
					a => BeginInvoke((MethodInvoker)(() => OnContactChanged(a))),
					ErrorManagement.ProcessException
					);
			}
		}

		private void OnContactDeleted(string contactId)
		{
			if (ContactDeleted != null)
				ContactDeleted(this, contactId);
		}

		private void OnContactChanged(Contact aContact)
		{
			if (ContactChanged != null)
				ContactChanged(this, aContact);
		}
	}
}
