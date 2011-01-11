using System;
using System.Web;
using LoveSeat.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using MindTouch.Dream;

namespace LoveSeat
{
	public static class PlugExtensions
	{
		public static Plug With(this Plug plug, ViewOptions options)
		{
			if (options == null)
				return plug;

			if ((options.Key != null) && (options.Key.Count > 0))
				plug.With("key", options.Key.ToString());
			if ((options.StartKey != null) && (options.StartKey.Count > 0))
				if ((options.StartKey.Count == 1) && (options.EndKey.Count > 1))
					plug.With("startkey",String.Format("[{0}]",options.StartKey.ToString()));
				else
					plug.With("startkey", options.StartKey.ToString());
			if ((options.EndKey != null) && (options.EndKey.Count > 0))
				plug.With("endkey", options.EndKey.ToString());
			if (options.Limit.HasValue)
				plug.With("limit", options.Limit.Value);
			if (options.Skip.HasValue)
				plug.With("skip", options.Skip.ToString()); 
			if (options.Reduce.HasValue)
				plug.With("reduce",options.Reduce.Value);
			if (options.Group.HasValue)
				plug.With("group",options.Group.Value);
			if (options.IncludeDocs.HasValue)
				plug.With("include_docs",options.IncludeDocs.Value);
			if (options.InclusiveEnd.HasValue)
				plug.With("inclusive_end",options.InclusiveEnd.Value);
			if (options.GroupLevel.HasValue)
				plug.With("group_level",options.GroupLevel.Value);
			if (options.Descending.HasValue)
				plug.With("descending",options.Descending.Value);
			if (options.Stale.HasValue && options.Stale.Value)
				plug.With("stale", "ok");
			if (!string.IsNullOrEmpty(options.StartKeyDocId))
				plug.With("startkey_docid",options.StartKeyDocId);
			if (!string.IsNullOrEmpty(options.EndKeyDocId))
				plug.With("endkey_docid", options.EndKeyDocId);
			if (!string.IsNullOrEmpty(options.Etag))
				plug = plug.WithHeader(DreamHeaders.IF_NONE_MATCH,options.Etag);
			return plug;
		}
	}

    public class ViewOptions : IViewOptions
    {
        public ViewOptions()
        {
            Key = new KeyOptions();
            StartKey = new KeyOptions();
            EndKey = new KeyOptions();
        }
       /// <summary>
        /// If you have a complex object as a string set this using a JRaw object()
        /// </summary>
        public IKeyOptions Key { get; set; }
        /// <summary>
        /// If you have a complex object as a string set this using a JRaw object()
        /// </summary>
        public IKeyOptions StartKey { get; set; }
        public string StartKeyDocId { get; set; }
        /// <summary>
        /// If you have a complex object as a string set this using a JRaw object()
        /// </summary>
        public IKeyOptions EndKey { get; set; }
        public string EndKeyDocId { get; set; }
        public int? Limit { get; set; }
        public int? Skip { get; set; }
        public bool? Reduce { get; set; }
        public bool? Group { get; set; }
        public bool? IncludeDocs { get; set; }
        public bool? InclusiveEnd { get; set; }
        public int? GroupLevel { get; set; }
        public bool? Descending { get; set; }
        public bool? Stale { get; set; }
        public string Etag { get; set; }


        public  override string ToString()
        {
            string result = "";
            if ((Key != null) && (Key.Count > 0))
                result += "&key=" + Key.ToString();
            if ((StartKey != null) && (StartKey.Count > 0))
                if((StartKey.Count == 1) && (EndKey.Count > 1))
                    result += "&startkey=[" + StartKey.ToString() + "]";
                else
                    result += "&startkey=" + StartKey.ToString();
            if ((EndKey != null) && (EndKey.Count > 0))
                result += "&endkey=" + EndKey.ToString();
            if (Limit.HasValue)
                result += "&limit=" + Limit.Value.ToString();
            if (Skip.HasValue)
                result += "&skip=" + Skip.Value.ToString();
            if (Reduce.HasValue)
                result += "&reduce=" + Reduce.Value.ToString().ToLower();
            if (Group.HasValue)
                result += "&group=" + Group.Value.ToString().ToLower();
            if (IncludeDocs.HasValue)
                result += "&include_docs=" + IncludeDocs.Value.ToString().ToLower();
            if (InclusiveEnd.HasValue)
                result += "&inclusive_end=" + InclusiveEnd.Value.ToString().ToLower();
            if (GroupLevel.HasValue)
                result += "&group_level=" + GroupLevel.Value.ToString();
            if (Descending.HasValue)
                result += "&descending=" + Descending.Value.ToString().ToLower();
            if (Stale.HasValue && Stale.Value)
                result += "&stale=ok";
            if (!string.IsNullOrEmpty(StartKeyDocId))
                result += "&startkey_docid=" + StartKeyDocId;
            if (!string.IsNullOrEmpty(EndKeyDocId))
                result += "&endkey_docid=" + EndKeyDocId;
            return result.Length < 1 ? "" :  "?" + result.Substring(1);
        }
    }
   

}