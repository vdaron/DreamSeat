using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MindTouch.Dream;

namespace LoveSeat.Support
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
			if (options.IncludeDocs.HasValue)
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
		public static Plug With(this Plug plug, ChangeOptions options)
		{
			switch (options.Feed)
			{
				case ChangeFeed.LongPoll:
				case ChangeFeed.Normal:
					plug = plug.With(Constants.FEED, Constants.FEED_NORMAL);
					break;
				case ChangeFeed.Continuous:
					plug = plug.With(Constants.FEED, Constants.FEED_CONTINUOUS);
					break;
				default:
					//Never get here
					break;
			}

			if (!String.IsNullOrEmpty(options.Filter))
			{
				plug = plug.With(Constants.FILTER, XUri.Encode(options.Filter));
			}
			if (options.Heartbeat.HasValue)
			{
				plug = plug.With(Constants.HEARTBEAT, options.Heartbeat.Value);
			}
			if (options.IncludeDocs.HasValue)
			{
				plug = plug.With(Constants.INCLUDE_DOCS, options.IncludeDocs.Value);
			}
			if (options.Limit.HasValue)
			{
				plug = plug.With(Constants.LIMIT, options.Limit.Value);
			}
			if (options.Since.HasValue)
			{
				plug = plug.With(Constants.SINCE, options.Since.Value);
			}
			if (options.Timeout.HasValue)
			{
				plug = plug.With(Constants.TIMEOUT, options.Timeout.Value);
			}

			return plug;
		}
	}
}
