using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoveSeat.Interfaces;

namespace LoveSeat
{
	public class ViewResultRow : IViewResultRow
	{
		public string Id
		{
			get;
			internal set;
		}

		public string Key
		{
			get;
			internal set;
		}
	}
	public class ViewResultRow<T> : ViewResultRow, IViewResultRow<T>
	{
		public T Value
		{
			get;
			internal set;
		}
	}
	public class ViewResultRow<T, U> : ViewResultRow<T>, IViewResultRow<T, U> where U : ICouchDocument
	{
		public U Doc
		{
			get;
			internal set;
		}
	}
}

