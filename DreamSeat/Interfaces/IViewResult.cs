using System.Collections.Generic;
using MindTouch.Dream;

namespace DreamSeat.Interfaces
{
	public interface IBaseViewResult
	{
		int TotalRows { get; }
		int OffSet { get; }
		string ETag { get; }
		DreamStatus Status { get; }
	}
	public interface IViewResult<TKey, TValue> : IBaseViewResult
	{
		IEnumerable<ViewResultRow<TKey, TValue>> Rows { get; }
	}
	public interface IViewResult<TKey, TValue, TDocument> : IBaseViewResult where TDocument : ICouchDocument
	{
		IEnumerable<ViewResultRow<TKey, TValue, TDocument>> Rows { get; }
	}
}
