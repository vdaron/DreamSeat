using System.Collections.Generic;
using System.Linq;
using DreamSeat.Interfaces;
using Newtonsoft.Json.Linq;

namespace DreamSeat
{
	public class JDocument : JObject, ICouchDocument
	{
		public string Id
		{
			get
			{
				JToken rev;
				return TryGetValue("_id", out rev) ? rev.Value<string>() : null;
			}
			set { this["_id"] = value; }
		}
		public string Rev
		{
			get
			{
				JToken rev;
				return TryGetValue("_rev", out rev) ? rev.Value<string>() : null;
			}
			set { this["_rev"] = value; }
		}
		public JDocument()
		{
		}
		public JDocument(JObject jobj)
			: base(jobj)
		{
		}
		public JDocument(string json)
			: base(Parse(json))
		{
		}

		public bool HasAttachment
		{
			get { return this["_attachments"] != null; }
		}

		public IEnumerable<string> GetAttachmentNames()
		{
			var attachment = this["_attachments"];
			return attachment == null ? null : attachment.Select(x => x.Value<JProperty>().Name);
		}
	}
}
