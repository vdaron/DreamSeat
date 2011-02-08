using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LoveSeat.Support
{
	public class LineReceivedEventArgs : EventArgs
	{
		public string Line { get; private set; }
		public LineReceivedEventArgs(string line)
		{
			Line = line;
		}
	}

	public class AsyncStreamReader : IDisposable
	{
		private readonly byte[] theReadBuffer = new byte[1];//TODO: Fix this.
		private readonly List<byte> theTempLineBytes = new List<byte>();
		private readonly EventHandler<LineReceivedEventArgs> theLineReaded;

		private bool isDisposed;
		private int theTempListIndex;

		public Stream BaseStream{get;private set;}
		public Encoding Encoding{get;private set;}

		public AsyncStreamReader(Stream stream, EventHandler<LineReceivedEventArgs> lineReceived):this(stream, Encoding.UTF8, lineReceived)
		{
		}
		public AsyncStreamReader(Stream stream, Encoding encoding, EventHandler<LineReceivedEventArgs> lineReceived)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (encoding == null)
				throw new ArgumentNullException("encoding");
			if (!stream.CanRead)
				throw new ArgumentException("Stream does not support reading");

			BaseStream = stream;
			Encoding = encoding;
			theLineReaded = lineReceived;
			BaseStream.BeginRead(theReadBuffer, 0, theReadBuffer.Length, AsyncCallback, null);
		}

		private void AsyncCallback(IAsyncResult ar)
		{
			try
			{
				int size = BaseStream.EndRead(ar);
				if (size > 0)
				{
					for(int i = 0; i < size; i++)
					{
						theTempLineBytes.Add(theReadBuffer[i]);
					}

					ReadLinesAndCallback();

					if (!isDisposed)
					{
						BaseStream.BeginRead(theReadBuffer, 0, theReadBuffer.Length, AsyncCallback, null);
					}
				}
			}
			catch
			{
				//TODO: ??
			}
		}
		private void ReadLinesAndCallback()
		{
			string line = ReadLine();
			while (!String.IsNullOrEmpty(line))
			{
				try
				{
					theLineReaded(this, new LineReceivedEventArgs(line));
				}
				catch
				{
					// TODO: Add Logging
				}
				line = ReadLine();
			}
		}
		private string ReadLine()
		{
			string line = null;
			int endLineIndex = 0;

			for (; theTempListIndex < theTempLineBytes.Count && endLineIndex == 0; theTempListIndex++)
			{
				if (theTempLineBytes[theTempListIndex] == '\n')
				{
					endLineIndex = theTempListIndex;
				}
			}

			if(endLineIndex > 0)
			{
				line = Encoding.GetString(theTempLineBytes.ToArray(), 0, endLineIndex);
				theTempLineBytes.RemoveRange(0,theTempListIndex);
				theTempListIndex = 0;
			}

			return line;
		}

		public void Dispose()
		{
			isDisposed = true;
			BaseStream.Dispose();
		}
	}
}
