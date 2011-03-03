using System;
using System.IO;
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
				changes.Since  = 1;
				try{
					if(File.Exists("sequence.txt")){
						changes.Since  = Int32.Parse(File.ReadAllText("sequence.txt"));
					}
				}catch(Exception e){
					Console.WriteLine("There was a problem while reading the sequence number from sequence.txt");
				}
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
			//record the sequence number of the change so when we run the application we
			//tell that we only need changes > this number. 
			//(Otherwise you get a change message for every doc in the db at the startup)
			
			try{
				File.WriteAllText("sequence.txt",r.Sequence.ToString());
			}catch(Exception e){
				Console.WriteLine("There was a problem while writing the sequence number to sequence.txt");
			}
			
			//verifiy type of change announced so we know if we need to refresh a contact
			if(r.Id.StartsWith("_design")){
				return;
			}
			//get the latest version of this document
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
