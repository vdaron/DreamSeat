using System.Collections.Generic;
using Newtonsoft.Json;

namespace LoveSeat
{
	public class ReplicationOptions : CouchDocument
	{
		[JsonProperty(Constants.SOURCE)]
		public string Source { get; set; }
		[JsonProperty(Constants.TARGET)]
		public string Target { get; set; }
		[JsonProperty(Constants.CONTINUOUS)]
		public bool? Continuous { get; set; }
		[JsonProperty(Constants.QUERY_PARAMS)]
		public Dictionary<string,string> QueryParams { get; set; }
		[JsonProperty(Constants.CREATE_TARGET)]
		public bool? CreateTarget { get; set; }
		[JsonProperty(Constants.FILTER)]
		public string Filter { get; set; }

		public ReplicationOptions(string source, string target)
		{
			Source = source;
			Target = target;
		}
	}
}