using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using LoveSeat.Interfaces;

namespace LoveSeat
{
	public class CouchUser : BaseDocument
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
	}
}