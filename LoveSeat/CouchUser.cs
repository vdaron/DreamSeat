using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace LoveSeat
{
	public class CouchUser : BaseDocument, ICouchDocument
	{
		private const string TYPE_USER = "user";

		public CouchUser()
		{
			Type = TYPE_USER;
		}

		[JsonProperty("type")]
		private string Type { get; set; }

		public string Name { get; set; }

		public string[] Roles { get; set; }

		////[JsonProperty("_id")]
		//public string Id { get; set; }
		////[JsonProperty("_rev")]
		//public string Rev { get; set; }
	}
}