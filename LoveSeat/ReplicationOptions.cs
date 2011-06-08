using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace LoveSeat
{
	public class ReplicationOptions
	{
		[JsonProperty(Constants.SOURCE)]
		public string Source { get; internal set; }
		[JsonProperty(Constants.TARGET)]
		public string Target { get; internal set; }
		[JsonProperty(Constants.CONTINUOUS)]
		public bool? Continuous { get; set; }
		[JsonProperty(Constants.QUERY_PARAMS)]
		public Dictionary<string,string> QueryParams { get; set; }
		[JsonProperty(Constants.CREATE_TARGET)]
		public bool? CreateTarget { get; set; }
		[JsonProperty(Constants.FILTER)]
		public string Filter { get; set; }

		[Obsolete("If using CouchDB >= 1.1 use CouchReplicationDocument")]
		public ReplicationOptions(string source, string target)
		{
			Source = source;
			Target = target;
		}

		public override string ToString()
		{
			JsonSerializerSettings settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
			return JsonConvert.SerializeObject(this, Formatting.None, settings);
		}
	}
}