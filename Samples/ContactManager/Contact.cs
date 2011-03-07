using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoveSeat;
using LoveSeat.Interfaces;

namespace ContactManager
{
	public class Contact : CouchDocument //, IAuditableDocument
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
		public DateTime CreationDate { get; private set; }
		public DateTime LastUpdateDate { get; private set; }

		public void Creating()
		{
			CreationDate = DateTime.Now;
			LastUpdateDate = DateTime.Now;
		}

		public void Updating()
		{
			LastUpdateDate = DateTime.Now;
		}

		public void Deleting()
		{
			
		}

		public void Created()
		{
			
		}

		public void Updated()
		{
			
		}

		public void Deleted()
		{
			
		}
	}
}
