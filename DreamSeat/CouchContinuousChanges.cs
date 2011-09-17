using System;
using DreamSeat.Interfaces;
using DreamSeat.Support;
using MindTouch.Dream;

namespace DreamSeat
{
	public delegate void CouchChangeDelegate(object sender, CouchChangeResult result);
	public delegate void CouchChangeDelegate<T>(object sender, CouchChangeResult<T> result) where T : ICouchDocument;

	public class CouchContinuousChanges : IDisposable
	{
		private readonly AsyncStreamReader theReader;
		private readonly ObjectSerializer<CouchChangeResult> theSerializer = new ObjectSerializer<CouchChangeResult>();

		internal CouchContinuousChanges(DreamMessage aMessage, CouchChangeDelegate aCallback)
		{
			theReader = new AsyncStreamReader(aMessage.ToStream(), (x, y) => {
				if (!String.IsNullOrEmpty(y.Line))
				{
					CouchChangeResult result = theSerializer.Deserialize(y.Line);
					aCallback(this, result);
				}
			});
		}

		public void Dispose()
		{
			theReader.Dispose();
		}
	}

	public class CouchContinuousChanges<T> : IDisposable where T : ICouchDocument
	{
		private readonly AsyncStreamReader theReader;
		private readonly ObjectSerializer<CouchChangeResult<T>> theSerializer = new ObjectSerializer<CouchChangeResult<T>>();

		internal CouchContinuousChanges(DreamMessage aMessage, CouchChangeDelegate<T> aCallback)
		{
			theReader = new AsyncStreamReader(aMessage.ToStream(), (x, y) => {
				if (!String.IsNullOrEmpty(y.Line))
				{
					CouchChangeResult<T> result = theSerializer.Deserialize(y.Line);
					aCallback(this, result);
				}
			});
		}

		public void Dispose()
		{
			theReader.Dispose();
		}
	}
}
