using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoveSeat
{
	public class CouchChanges
	{
		[JsonProperty(Constants.RESULTS)]
		public CouchChangeResult[] Results { get; internal set; }
		[JsonProperty(Constants.LAST_SEQUENCE)]
		public int LastSeq { get; internal set; }
	}

	public class CouchChangeResult
	{
		[JsonProperty(Constants.ID)]
		public string Id { get; private set; }
		[JsonProperty(Constants.SEQUENCE)]
		public int Sequence { get; private set; }
		[JsonProperty(Constants.CHANGES)]
		public JObject[] Changes { get; private set; }
	}
}
