using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using LoveSeat.Interfaces;

namespace LoveSeat
{
	public class CouchUser : CouchDocument
	{
		private const string TYPE_USER = "user";

		public CouchUser()
		{
			Type = TYPE_USER;
		}

		[JsonProperty("type")]
		private string Type { get; set; }
		[JsonProperty("name")]
		public string Name { get; set; }
		[JsonProperty("roles")]
		public string[] Roles { get; set; }
	}
}