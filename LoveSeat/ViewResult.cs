using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using LoveSeat.Interfaces;
using LoveSeat.Support;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MindTouch.Dream;

namespace LoveSeat
{
    public class ViewResult<T> : ViewResult
    {
        private readonly IObjectSerializer<T> objectSerializer = null;
        public ViewResult(DreamMessage message, IObjectSerializer<T> objectSerializer)
            : base(message)
        {
            this.objectSerializer = objectSerializer;
        }

        public IEnumerable<T> Items
        {
            get
            {
                if (objectSerializer == null)
                {
                    throw new InvalidOperationException("ObjectSerializer must be set in order to use the generic view.");
                }
                return this.RawValues.Select(item => objectSerializer.Deserialize(item));
            }
        }
    }

    public class ViewResult : IViewResult
    {
        private JObject json = null;
		private DreamMessage message;
        private readonly string responseString;

        public JObject Json { get { return json ?? (json = JObject.Parse(responseString)); } }
        public ViewResult(DreamMessage message)
        {
			this.message = message;
			this.responseString = message.ToText();
        }
        public DreamStatus StatusCode { get { return message.Status; } }

        public string Etag { get { return message.Headers["ETag"]; } }
        public int TotalRows { get
        {
            if (Json["total_rows"] == null) throw new CouchException(message, Json["reason"].Value<string>());
            return Json["total_rows"].Value<int>();
        } }
        public int OffSet { get
        {
            if (Json["offset"] == null) throw new CouchException(message, Json["reason"].Value<string>());
            return Json["offset"].Value<int>();
        } }
        public IEnumerable<JToken> Rows { get
        {
            if (Json["rows"] == null) throw new CouchException(message, Json["reason"].Value<string>());
            return (JArray)Json["rows"];
        } }
        /// <summary>
        /// Only populated when IncludeDocs is true
        /// </summary>
        public IEnumerable<JToken> Docs
        {
            get
            {
                return (JArray)Json["doc"];
            }
        }
        /// <summary>
        /// An IEnumerable of strings instead of the IEnumerable of JTokens
        /// </summary>
        public IEnumerable<string> RawRows
        {
            get
            {
                var arry = (JArray)Json["rows"];
                return arry.Select(item => item.ToString());
            }
        }
        public IEnumerable<string> RawValues
        {
            get
            {
                var arry = (JArray)Json["rows"];
                return arry.Select(item => item["value"].ToString());
            }
        }
        public IEnumerable<string> RawDocs
        {
            get
            {
                var arry = (JArray)Json["rows"];
                return arry.Select(item => item["doc"].ToString());
            }
        }
        public string RawString
        {
            get { return responseString; }
        }

        public bool Equals(IListResult other)
        {
            if (string.IsNullOrEmpty(Etag) || string.IsNullOrEmpty(other.Etag)) return false;
            return Etag == other.Etag;
        }

        public override string ToString()
        {
            return responseString;
        }
        /// <summary>
        /// Provides a formatted version of the json returned from this Result.  (Avoid this method in favor of RawString as it's much more performant)
        /// </summary>
        public string FormattedResponse { get { return Json.ToString(Formatting.Indented); } }
    }
}