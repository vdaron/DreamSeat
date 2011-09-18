using Newtonsoft.Json;

namespace DreamSeat
{
	public class CouchView
	{
		public CouchView(string aMap = null, string aReduce = null)
		{
			Map = aMap;
			Reduce = aReduce;
		}

		[JsonProperty("map")]
		public string Map { get; set; }
		[JsonProperty("reduce")]
		public string Reduce { get; set; }
	}
}
