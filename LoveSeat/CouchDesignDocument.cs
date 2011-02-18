using System.Collections.Generic;
using Newtonsoft.Json;

namespace LoveSeat
{
	public class CouchDesignDocument : CouchDocument
	{
		public CouchDesignDocument()
		{
			Language = Constants.JAVASCRIPT;
			Views = new Dictionary<string, CouchView>();
		}
		public CouchDesignDocument(string id)
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
