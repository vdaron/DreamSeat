using Newtonsoft.Json;

namespace LoveSeat
{
	public class CouchAttachment
	{
		[JsonProperty(Constants.STUB)]
		public bool Stub{get;internal set;}
		[JsonProperty(Constants.CONTENT_TYPE)]
		public string ContentType{get;internal set;}
		[JsonProperty(Constants.LENGTH)]
		public long Length{get;internal set;}
		[JsonProperty(Constants.DATA)]
		public string Data { get; internal set; }
	}
}
