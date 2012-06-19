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
				return TryGetValue(Constants._ID, out rev) ? rev.Value<string>() : null;
			}
			set { this[Constants._ID] = value; }
		}
		public string Rev
		{
			get
			{
				JToken rev;
				return TryGetValue(Constants._REV, out rev) ? rev.Value<string>() : null;
			}
			set { this[Constants._REV] = value; }
		}
		public JDocument()
		{
		}
		public JDocument(JObject aJobj)
			: base(aJobj)
		{
		}
		public JDocument(string aJson)
			: base(Parse(aJson))
		{
		}

		public bool HasAttachment
		{
			get { return this[Constants.ATTACHMENTS] != null; }
		}

		public IEnumerable<string> GetAttachmentNames()
		{
			var attachment = this[Constants.ATTACHMENTS];
			return attachment == null ? null : attachment.Select(x => x.Value<JProperty>().Name);
		}
	}
}
