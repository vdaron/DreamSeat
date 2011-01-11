using System;
using System.Net;
using LoveSeat.Interfaces;
using LoveSeat.Support;
using MindTouch.Dream;

namespace LoveSeat
{
    public class ListResult : IListResult
    {
        private readonly DreamMessage message;

        public ListResult(DreamMessage message)
        {
            this.message = message;
        }

        public DreamStatus StatusCode
        {
            get { return message.Status; }
        }

        public string Etag
        {
            get { return message.Headers["ETag"]; }
        }

        public string RawString
        {
            get { return message.ToText(); }
        }

        public bool Equals(IListResult other)
        {
            if (other == null)
                return false;

            if (string.IsNullOrEmpty(other.Etag))
                return false;

            return other.Etag == Etag;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IListResult);
        }

        public override int GetHashCode()
        {
            if (string.IsNullOrEmpty(Etag))
                return base.GetHashCode();

            return Etag.GetHashCode();
        }
    }
}