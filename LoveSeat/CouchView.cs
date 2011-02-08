using Newtonsoft.Json;

namespace LoveSeat
{
	public class CouchView
	{
		internal CouchView() { }

		public CouchView(string map)
			:this(map,null)
		{}
		public CouchView(string map, string reduce)
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
