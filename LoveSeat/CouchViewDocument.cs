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
			Language = Constants.JAVASCRIPT;
			Views = new Dictionary<string, CouchView>();
		}
		public CouchViewDocument(string id)
			:this()
		{
			Id = Constants.DESIGN + "/" + id;
		}

		[JsonProperty(Constants.LANGUAGE)]
		public string Language{get;set;}

		[JsonProperty(Constants.VIEWS)]
		public Dictionary<string, CouchView> Views { get; internal set; }
	}
}
