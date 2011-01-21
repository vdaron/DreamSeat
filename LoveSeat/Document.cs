using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using LoveSeat.Interfaces;

namespace LoveSeat
{
	public class CouchDocument : ICouchDocument
	{
		public CouchDocument()
		{
			Attachments = new Dictionary<string, CouchAttachment>();
		}

		[JsonProperty("_id")]
		public string Id { get; set; }
		[JsonProperty("_rev")]
		public string Rev { get; set; }

		[JsonProperty("_attachments")]
		private Dictionary<string, CouchAttachment> Attachments;

		[JsonIgnore]
		public bool HasAttachment { get { return Attachments.Count > 0; } }

		public IEnumerable<string> GetAttachmentNames()
		{
			foreach (string key in Attachments.Keys)
				yield return key;
			yield break;
		}
	}
}