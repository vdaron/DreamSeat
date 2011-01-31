using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MindTouch.Dream;
using LoveSeat.Support;
using LoveSeat.Interfaces;

namespace LoveSeat
{
	public delegate void CouchChangeDelegate(object sender, CouchChangeResult result);
	public delegate void CouchChangeDelegate<T>(object sender, CouchChangeResult<T> result) where T : ICouchDocument;

	public class CouchContinuousChanges : IDisposable
	{
		private AsyncStreamReader theReader;
		private ObjectSerializer<CouchChangeResult> theSerializer = new ObjectSerializer<CouchChangeResult>();

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
		private AsyncStreamReader theReader;
		private ObjectSerializer<CouchChangeResult<T>> theSerializer = new ObjectSerializer<CouchChangeResult<T>>();

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
