using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace DreamSeat
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
		public ReplicationOptions(string aSource, string aTarget)
		{
			if (aSource == null)
				throw new ArgumentNullException("aSource");
			if (aTarget == null)
				throw new ArgumentNullException("aTarget");

			Source = aSource;
			Target = aTarget;
		}

		public override string ToString()
		{
			JsonSerializerSettings settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
			return JsonConvert.SerializeObject(this, Formatting.None, settings);
		}
	}
}