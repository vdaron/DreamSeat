using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoveSeat;

namespace ContactManager
{
	public class Contact : CouchDocument//, IComparable, IComparer<Contact>
	{
		public Contact()
		{
			Type = "contact";
			EmailAddresses = new List<string>();
		}

		public string Type { get; private set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public List<string> EmailAddresses { get; set; }
		
		/*public int CompareTo(object o){
			Console.WriteLine("Theeere");
			Contact aContact = o as Contact;
			if(this.LastName.CompareTo(aContact.LastName)!=0)
				return this.LastName.CompareTo(aContact.LastName);
			else if(this.FirstName.CompareTo(aContact.FirstName)!=0)
				return this.FirstName.CompareTo(aContact.FirstName);
			else return this.Id.CompareTo(aContact.Id);	                                                    
		}
		
		public int Compare(Contact x, Contact y){
			Console.WriteLine("lallalala");
			if(x.LastName.CompareTo(y.LastName)!=0)
				return x.LastName.CompareTo(y.LastName);
			else if(x.FirstName.CompareTo(y.FirstName)!=0)
				return x.FirstName.CompareTo(y.FirstName);
			else return x.Id.CompareTo(y.Id);	
		}*/
		
	}
}
