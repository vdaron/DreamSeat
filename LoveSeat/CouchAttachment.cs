using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace LoveSeat
{
	public class CouchAttachment
	{
		[JsonProperty("stub")]
		public bool Stub{get;internal set;}
		[JsonProperty("content_type")]
		public string ContentType{get;internal set;}
		[JsonProperty("length")]
		public long Length{get;internal set;}
		[JsonProperty("data")]
		public string Data { get; internal set; }
	}
}
