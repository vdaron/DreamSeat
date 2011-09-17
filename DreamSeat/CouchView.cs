using Newtonsoft.Json;

namespace DreamSeat
{
	public class CouchView
	{
		public CouchView(string map = null, string reduce = null)
		{
			Map = map;
			Reduce = reduce;
		}

		[JsonProperty("map")]
		public string Map { get; set; }
		[JsonProperty("reduce")]
		public string Reduce { get; set; }
	}
}
