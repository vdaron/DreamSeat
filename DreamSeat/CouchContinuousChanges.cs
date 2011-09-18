using System;
using DreamSeat.Interfaces;
using DreamSeat.Support;
using MindTouch.Dream;

namespace DreamSeat
{
	public delegate void CouchChangeDelegate(object aSender, CouchChangeResult aResult);
	public delegate void CouchChangeDelegate<T>(object aSender, CouchChangeResult<T> aResult) where T : ICouchDocument;

	public class CouchContinuousChanges : IDisposable
	{
		private readonly AsyncStreamReader theReader;
		private readonly ObjectSerializer<CouchChangeResult> theSerializer = new ObjectSerializer<CouchChangeResult>();

		internal CouchContinuousChanges(DreamMessage aMessage, CouchChangeDelegate aCallback)
		{
			if (aMessage == null)
				throw new ArgumentNullException("aMessage");
			if (aCallback == null)
				throw new ArgumentNullException("aCallback");

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
			if (aMessage == null)
				throw new ArgumentNullException("aMessage");
			if (aCallback == null)
				throw new ArgumentNullException("aCallback");

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
