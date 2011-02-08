using MindTouch.Dream;

namespace LoveSeat
{
	public class CouchException : System.Exception
	{
		private readonly DreamMessage theMessage;

		public CouchException(DreamMessage msg)
			: this(msg, msg.ToText())
		{ }
		public CouchException(DreamMessage msg, string mesg)
			: base(mesg)
		{
			theMessage = msg;
		}
		public DreamStatus Status { get { return theMessage.Status; } }
		public DreamMessage DreamMessage { get { return theMessage; } }
	}
}
