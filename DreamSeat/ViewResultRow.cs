using DreamSeat.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DreamSeat
{
	public class ViewResultRow<TKey> : IViewResultRow<TKey>
	{
		[JsonProperty(Constants.ID)]
		public string Id
		{
			get;
			internal set;
		}
		[JsonProperty(Constants.KEY)]
		public TKey Key
		{
			get;
			internal set;
		}
	}
	public class ViewResultRow<TKey, TValue> : 
		ViewResultRow<TKey>, 
		IViewResultRow<TKey,TValue>
	{
		[JsonProperty(Constants.VALUE)]
		public TValue Value
		{
			get;
			internal set;
		}
	}
	public class ViewResultRow<TKey, TValue, TDocument> : 
		ViewResultRow<TKey,TValue>, 
		IViewResultRow<TKey, TValue, TDocument> 
		where TDocument : ICouchDocument
	{
		[JsonProperty(Constants.DOC)]
		public TDocument Doc
		{
			get;
			internal set;
		}
	}
}

