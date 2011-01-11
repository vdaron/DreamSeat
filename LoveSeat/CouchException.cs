using System.Net;
using MindTouch.Dream;

namespace LoveSeat
{
   public  class CouchException : System.Exception
    {
       private readonly DreamMessage message;

	   public CouchException(DreamMessage msg)
		   :this(msg,msg.ToText())
	   { }
       public CouchException(DreamMessage msg, string mesg) : base(mesg)
       {
           this.message = msg;
       }
	   public DreamStatus Status {get{return message.Status;}}
       public DreamMessage DreamMessage { get { return message; } }
    }
}
