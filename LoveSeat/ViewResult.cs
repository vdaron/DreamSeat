using System.Collections.Generic;
using LoveSeat.Interfaces;
using MindTouch.Dream;
using Newtonsoft.Json;

namespace LoveSeat
{
	public abstract class BaseViewResult : IBaseViewResult
	{
		[JsonProperty("total_rows")]
		public int TotalRows
		{
			get;
			internal set;
		}
		[JsonProperty("offset")]
		public int OffSet
		{
			get;
			internal set;
		}
		[JsonIgnore]
		public string ETag { get; internal set; }

		[JsonIgnore]
		public DreamStatus Status { get; internal set; }
	}
	public class ViewResult<TKey, TValue> : BaseViewResult, IViewResult<TKey, TValue>
	{
		[JsonProperty("rows")]
		public IEnumerable<ViewResultRow<TKey,TValue>> Rows
		{
			get;
			internal set;
		}
	}
	public class ViewResult<TKey, TValue, TDocument> : BaseViewResult, IViewResult<TKey, TValue, TDocument> where TDocument : ICouchDocument
	{
		[JsonProperty("rows")]
		public IEnumerable<ViewResultRow<TKey, TValue,TDocument>> Rows
		{
			get;
			internal set;
		}
	}
}