using System.Collections.Generic;
using DreamSeat.Interfaces;
using Newtonsoft.Json;
using System;
using DreamSeat;

namespace DreamSeat
{
	public class CouchDocument : ICouchDocument
	{
		public CouchDocument()
		{
			Attachments = new Dictionary<string, CouchAttachment>();
		}

		[JsonProperty(Constants._ID)]
		public string Id { get; set; }
		[JsonProperty(Constants._REV)]
		public string Rev { get; set; }

		[JsonProperty(Constants.ATTACHMENTS)]
		internal Dictionary<string, CouchAttachment> Attachments;

		[JsonIgnore]
		public bool HasAttachment { get { return Attachments.Count > 0; } }

		public IEnumerable<string> GetAttachmentNames()
		{
			foreach (string key in Attachments.Keys)
				yield return key;
			yield break;
		}

		public override bool Equals(object obj)
		{
			ICouchDocument o = obj as ICouchDocument;
			if(o == null)
				return false;

			return o.Id == Id && o.Rev == Rev;
		}

		public override int GetHashCode()
		{
			//TODO: check this
			return (Id + Rev).GetHashCode();
		}
	}
}