using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LoveSeat.Interfaces
{
	public interface IBaseViewResult
	{
		int TotalRows { get; }
		int OffSet { get; }
		string ETag { get; }
	}
	public interface IViewResult<T> : IBaseViewResult
	{
		IEnumerable<ViewResultRow<T>> Rows { get; }
	}
	public interface IViewResult<T, U> : IBaseViewResult where U : ICouchDocument
	{
		IEnumerable<ViewResultRow<T, U>> Rows { get; }
	}
}
