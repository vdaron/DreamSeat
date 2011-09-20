using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DreamSeat
{
	public class UserContext
	{
		[JsonProperty("name")]
		public string Name { get; set; }
		[JsonProperty("roles")]
		public string[] Roles { get; set; }
	}

	public class CouchReplicationDocument : CouchDocument
	{
		public CouchReplicationDocument()
		{
			QueryParams = new Dictionary<string, string>();
		}

		[JsonProperty(Constants.SOURCE)]
		public string Source { get; set; }
		[JsonProperty(Constants.TARGET)]
		public string Target { get; set; }
		[JsonProperty(Constants.CONTINUOUS)]
		public bool Continuous { get; set; }
		[JsonProperty(Constants.QUERY_PARAMS)]
		public Dictionary<string, string> QueryParams { get; set; }
		[JsonProperty(Constants.CREATE_TARGET)]
		public bool? CreateTarget { get; set; }
		[JsonProperty(Constants.FILTER)]
		public string Filter { get; set; }
		[JsonProperty(Constants.USER_CONTEXT)]
		public UserContext UserContext { get; set; }
		[JsonProperty(Constants.DOC_IDS)]
		public string[] DocIds { get; set; }

		[JsonProperty(Constants.REPLICATION_ID)]
		public string ReplicationId { get; internal set; }
		[JsonProperty(Constants.REPLICATION_STATE)]
		public string ReplicationState { get; internal set; }
		[JsonProperty(Constants.REPLICATION_STATE_TIME)]
		public DateTimeOffset? ReplicationStateTime { get; internal set; }
	}
}
