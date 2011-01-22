using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using LoveSeat.Interfaces;
using LoveSeat.Support;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MindTouch.Dream;

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
	public class ViewResult<T> : BaseViewResult, IViewResult<T>
	{
		[JsonProperty("rows")]
		public IEnumerable<ViewResultRow<T>> Rows
		{
			get;
			internal set;
		}
	}
	public class ViewResult<T, U> : BaseViewResult, IViewResult<T, U> where U : ICouchDocument
	{
		[JsonProperty("rows")]
		public IEnumerable<ViewResultRow<T, U>> Rows
		{
			get;
			internal set;
		}
	}
}