using Newtonsoft.Json;
using System.Collections.Generic;

namespace LoveSeat
{
	public class ReplicationOptions
	{
		[JsonProperty("source")]
		public string Source { get; internal set; }
		[JsonProperty("target")]
		public string Target { get; internal set; }
		[JsonProperty("continuous")]
		public bool? Continuous { get; set; }
		[JsonProperty("query_params")]
		public Dictionary<string,string> QueryParams { get; set; }
		[JsonProperty("create_target")]
		public bool? CreateTarget { get; set; }
		[JsonProperty("filter")]
		public string Filter { get; set; }

		public ReplicationOptions(string source, string target)
		{
			Source = source;
			Target = target;
		}

		public override string ToString()
		{
			JsonSerializerSettings settings = new JsonSerializerSettings();
			settings.NullValueHandling = NullValueHandling.Ignore;
			return JsonConvert.SerializeObject(this, Formatting.None, settings);
		}
	}
}