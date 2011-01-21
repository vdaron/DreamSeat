using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoveSeat.Interfaces
{
	public interface IViewResultRow
	{
		string Id { get; }
		string Key { get; }
	}
	public interface IViewResultRow<T> : IViewResultRow
	{
		T Value { get; }
	}
	public interface IViewResultRow<T, U> : IViewResultRow<T> where U : ICouchDocument
	{
		U Doc { get; }
	}
}
