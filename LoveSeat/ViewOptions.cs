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
				plug = plug.With(Constants.KEY, options.Key.ToString());
			if ((options.StartKey != null) && (options.StartKey.Count > 0))
				if ((options.StartKey.Count == 1) && (options.EndKey.Count > 1))
					plug = plug.With(Constants.STARTKEY, String.Format("[{0}]", options.StartKey.ToString()));
				else
					plug = plug.With(Constants.STARTKEY, options.StartKey.ToString());
			if ((options.EndKey != null) && (options.EndKey.Count > 0))
				plug = plug.With(Constants.ENDKEY, options.EndKey.ToString());
			if (options.Limit.HasValue)
				plug = plug.With(Constants.LIMIT, options.Limit.Value);
			if (options.Skip.HasValue)
				plug = plug.With(Constants.SKIP, options.Skip.ToString());
			if (options.Reduce.HasValue)
				plug = plug.With(Constants.REDUCE, options.Reduce.Value);
			if (options.Group.HasValue)
				plug = plug.With(Constants.GROUP, options.Group.Value);
			if (options.InclusiveEnd.HasValue)
				plug = plug.With(Constants.INCLUSIVE_END, options.InclusiveEnd.Value);
			if(options.IncludeDocs.HasValue)
				plug = plug.With(Constants.INCLUDE_DOCS, options.IncludeDocs.Value);
			if (options.GroupLevel.HasValue)
				plug = plug.With(Constants.GROUP_LEVEL, options.GroupLevel.Value);
			if (options.Descending.HasValue)
				plug = plug.With(Constants.DESCENDING, options.Descending.Value);
			if (options.Stale.HasValue && options.Stale.Value)
				plug = plug.With(Constants.STALE, Constants.OK);
			if (!string.IsNullOrEmpty(options.StartKeyDocId))
				plug = plug.With(Constants.STARTKEY_DOCID, options.StartKeyDocId);
			if (!string.IsNullOrEmpty(options.EndKeyDocId))
				plug = plug.With(Constants.ENDKEY_DOCID, options.EndKeyDocId);
			if (!string.IsNullOrEmpty(options.Etag))
				plug = plug.WithHeader(DreamHeaders.IF_NONE_MATCH, options.Etag);
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
	}
}