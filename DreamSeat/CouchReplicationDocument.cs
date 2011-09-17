using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DreamSeat
{
	public class CouchReplicationDocument : CouchDocument
	{
		private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0,DateTimeKind.Utc);

		[JsonProperty(Constants.SOURCE)]
		public string Source { get; set; }
		[JsonProperty(Constants.TARGET)]
		public string Target { get; set; }
		[JsonProperty(Constants.CONTINUOUS)]
		public bool? Continuous { get; set; }
		[JsonProperty(Constants.QUERY_PARAMS)]
		public Dictionary<string, string> QueryParams { get; set; }
		[JsonProperty(Constants.CREATE_TARGET)]
		public bool? CreateTarget { get; set; }
		[JsonProperty(Constants.FILTER)]
		public string Filter { get; set; }

		[JsonProperty(Constants.REPLICATION_ID)]
		public string ReplicationId { get; internal set; }
		[JsonProperty(Constants.REPLICATION_STATE)]
		public string ReplicationState { get; internal set; }
		[JsonProperty(Constants.REPLICATION_STATE_TIME)]
		public DateTimeOffset? ReplicationStateTime { get; internal set; }
	}
}
