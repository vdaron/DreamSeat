using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MindTouch.Dream;
using LoveSeat.Support;

namespace LoveSeat
{
	public delegate void CouchChangeDelegate(object sender, CouchChangeResult result);

	public class CouchContinuousChanges : IDisposable
	{
		public event CouchChangeDelegate OnChange;

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
}
