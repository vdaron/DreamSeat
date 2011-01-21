using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace LoveSeat
{
	public class CouchViewDocument : CouchDocument
	{
		public CouchViewDocument()
		{
			Language = "javascript";
			Views = new Dictionary<string, CouchView>();
		}
		public CouchViewDocument(string id)
			:this()
		{
			Id = "_design/" + id;
		}

		[JsonProperty("language")]
		public string Language{get;set;}

		[JsonProperty("views")]
		public Dictionary<string, CouchView> Views { get; internal set; }
	}
}
