using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Globalization;
using MindTouch.IO;

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
		private byte[] theAsyncBuffer = new byte[1024];//TODO: Fix this.
		private IAsyncResult theAsyncResult;
		private List<char> theCharList = new List<char>();
		private Decoder theDecoder;
		private StringBuilder theCurrentLine = new StringBuilder();
		private EventHandler<LineReceivedEventArgs> theLineReaded;
		private bool isDisposed = false;

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
			theDecoder = encoding.GetDecoder();
			theLineReaded = lineReceived;
			theAsyncResult = BaseStream.BeginRead(theAsyncBuffer, 0, theAsyncBuffer.Length, AsyncCallback, null);
		}

		private void AsyncCallback(IAsyncResult ar)
		{
			try
			{
				int size = BaseStream.EndRead(ar);
				if (size > 0)
				{
					char[] charArray = new char[theDecoder.GetCharCount(theAsyncBuffer, 0, size)];
					theDecoder.GetChars(theAsyncBuffer, 0, size, charArray, 0, false);

					foreach (char c in charArray)
						theCharList.Add(c);

					ReadLinesAndCallback();

					if (!isDisposed)
					{
						theAsyncResult = BaseStream.BeginRead(theAsyncBuffer, 0, theAsyncBuffer.Length, AsyncCallback, null);
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
			while (ReadLineFromStack(theCurrentLine))
			{
				try
				{
					theLineReaded(this, new LineReceivedEventArgs(theCurrentLine.ToString()));
				}
				catch
				{
					// TODO: Add Logging
				}
				theCurrentLine.Length = 0;
			}
		}
		private bool ReadLineFromStack(StringBuilder builder)
		{
			bool newLine = false;
			while(!newLine && theCharList.Count > 0)
			{
				char ch = theCharList[0];
				theCharList.RemoveAt(0);
				switch (ch)
				{
					case '\r':
						if (theCharList.Count > 0 && theCharList[0] == '\n')
							theCharList.RemoveAt(0);
						newLine = true;
						break;
					case '\n':
						newLine = true;
						break;
					default:
						builder.Append(ch);
						break;
				}
			}
			return newLine;
		}

		public void Dispose()
		{
			isDisposed = true;
			BaseStream.Dispose();
		}
	}

}
