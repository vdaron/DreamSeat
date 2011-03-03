using System.Collections.Generic;
using LoveSeat.Interfaces;
using Newtonsoft.Json;
using System;
using LoveSeat;

namespace LoveSeat
{
	public class CouchDocument : ICouchDocument, IEquatable<CouchDocument>, IComparer<CouchDocument>
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
		
		public bool Equals(CouchDocument aDocument){
			Console.WriteLine(this.Id.Equals(aDocument.Id));
			return this.Id.Equals(aDocument.Id);
		}
		
		public int Compare(CouchDocument x, CouchDocument y){
			Console.WriteLine(x.Id.CompareTo(y.Id));
			return x.Id.CompareTo(y.Id);
		}
	
	}
}