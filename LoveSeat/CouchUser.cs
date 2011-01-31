using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using LoveSeat.Interfaces;

namespace LoveSeat
{
	public class CouchUser : CouchDocument
	{
		public CouchUser()
		{
			Type = Constants.TYPE_USER;
			Roles = new string[0];
		}

		[JsonProperty(Constants.TYPE)]
		private string Type { get; set; }
		[JsonProperty(Constants.NAME)]
		public string Name { get; set; }
		[JsonProperty(Constants.ROLES)]
		public string[] Roles { get; set; }
	}
}