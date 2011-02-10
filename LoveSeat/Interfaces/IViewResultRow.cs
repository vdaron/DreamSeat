using Newtonsoft.Json.Linq;
namespace LoveSeat.Interfaces
{
	public interface IViewResultRow<TKey>
	{
		string Id { get; }
		TKey Key { get; }
	}
	public interface IViewResultRow<TKey,TValue> : IViewResultRow<TKey>
	{
		TValue Value { get; }
	}
	public interface IViewResultRow<TKey, TValue, TDocument> : IViewResultRow<TKey,TValue> where TDocument : ICouchDocument
	{
		TDocument Doc { get; }
	}
}
