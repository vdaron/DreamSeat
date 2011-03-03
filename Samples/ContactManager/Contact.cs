using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoveSeat;

namespace ContactManager
{
	public class Contact : CouchDocument
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
		
	}
}
