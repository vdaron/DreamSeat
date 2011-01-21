using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using LoveSeat.Interfaces;

namespace LoveSeat
{
	public class JDocument : JObject, ICouchDocument
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
		public JDocument()
		{
		}
		public JDocument(JObject jobj)
			: base(jobj)
		{
		}
		public JDocument(string json)
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
}
