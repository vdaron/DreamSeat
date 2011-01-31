using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LoveSeat.Interfaces;

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
		public string Id { get; protected set; }
		[JsonProperty(Constants.SEQUENCE)]
		public int Sequence { get; protected set; }
		[JsonProperty(Constants.CHANGES)]
		public JObject[] Changes { get; protected set; }
	}

	public class CouchChanges<T>  where T : ICouchDocument
	{
		[JsonProperty(Constants.RESULTS)]
		public CouchChangeResult<T>[] Results { get; internal set; }
		[JsonProperty(Constants.LAST_SEQUENCE)]
		public int LastSeq { get; internal set; }

	}

	public class CouchChangeResult<T> : CouchChangeResult where T : ICouchDocument
	{
		public T Doc { get; private set; }
	}
}
