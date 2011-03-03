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
	
	public delegate void ContactChangedDelegate(object aSender, Contact aContact);
	
	public partial class ChangesListBox : ListBox
	{
		private CouchDatabase theDatabase;
		public event ContactChangedDelegate ContactChanged;
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
				theContinuousChangeManager = theDatabase.GetCoutinuousChanges(changes, (x, y) => BeginInvoke((MethodInvoker)(() => changement(y))));
			}
		}

		private static void ChangesListBoxFormat(object sender, ListControlConvertEventArgs e)
		{
			CouchChangeResult change = e.ListItem as CouchChangeResult;
			if((change != null)&&(change.Changes.Length >= 1))
			{
				e.Value = String.Format("{0:0000}\t{1}\t{2}", change.Sequence, change.Id, change.Changes[0].ToString());
			}
		}
		
		private void changement(CouchChangeResult r){
			//add to the list of changes
			Items.Add(r);
			//verifiy type of change announced
			Coroutine.Invoke(ContactChangedLoader, r.Id, new Result<Contact>()).WhenDone(
				a => BeginInvoke((MethodInvoker)(() => ContactChanged(this,a))),
				ErrorManagement.ProcessException
				);
		}
		
		public Yield ContactChangedLoader(string id, Result<Contact> aResult){
			var docRes = new Result<Contact>();
			yield return docRes = theDatabase.GetDocument<Contact>(id,docRes);

			if(docRes.HasException)
			{
				aResult.Throw(docRes.Exception);
				yield break;
			}
			Console.WriteLine("### docRes.Value: "+docRes.Value+" ###");
			aResult.Return(docRes.Value);
		}
	}
}
