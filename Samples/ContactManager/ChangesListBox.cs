using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LoveSeat;

namespace ContactManager
{
	public partial class ChangesListBox : ListBox
	{
		private CouchDatabase theDatabase;
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
			if(theDatabase != null)
			{
				Items.Clear();
				ChangeOptions changes = new ChangeOptions();
				changes.Heartbeat = 10000;
				theContinuousChangeManager = theDatabase.GetCoutinuousChanges(changes, (x, y) => BeginInvoke((MethodInvoker)(() => Items.Add(y))));
			}
		}

		private static void ChangesListBoxFormat(object sender, ListControlConvertEventArgs e)
		{
			CouchChangeResult change = e.ListItem as CouchChangeResult;
			e.Value = change != null ? String.Format("{0:0000}\t{1}\t{2}", change.Sequence, change.Id, change.Changes.ToString()) : e.ListItem.ToString();
		}
	}
}
