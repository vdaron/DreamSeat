using LoveSeat.Interfaces;
using Newtonsoft.Json.Linq;

namespace LoveSeat
{
	public class ViewResultRow<TKey> : IViewResultRow<TKey>
	{
		public string Id
		{
			get;
			internal set;
		}

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
		public TDocument Doc
		{
			get;
			internal set;
		}
	}
}

