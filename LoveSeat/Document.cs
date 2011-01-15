using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace LoveSeat
{
	public interface ICouchDocument
	{
		string Id { get; set; }
		string Rev { get; set; }
	}

	public class JsonDocument : JObject, ICouchDocument
	{
		public string Id
		{
			get
			{
				JToken rev;
				if (this.TryGetValue("_id", out rev))
				{
					return rev.Value<string>();
				}
				return null;
			}
			set { this["_id"] = value; }
		}
		public string Rev
		{
			get
			{
				JToken rev;
				if (this.TryGetValue("_rev", out rev))
				{
					return rev.Value<string>();
				}
				return null;
			}
			set { this["_rev"] = value; }
		}
		protected JsonDocument()
		{
		}
		public JsonDocument(JObject jobj)
			: base(jobj)
		{
		}
		public JsonDocument(string json)
			: base(JObject.Parse(json))
		{
		}
		public bool HasAttachment
		{
			get { return this["_attachments"] != null; }
		}

		public IEnumerable<string> GetAttachmentNames()
		{
			var attachment = this["_attachments"];
			if (attachment == null) return null;
			return attachment.Select(x => x.Value<JProperty>().Name);
		}
	}

	public class BaseDocument : ICouchDocument
	{
		[JsonProperty("_id")]
		public string Id { get; set; }
		[JsonProperty("_rev")]
		public string Rev { get; set; }
	}
}