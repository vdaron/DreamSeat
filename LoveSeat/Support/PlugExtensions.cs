using System;
using MindTouch.Dream;

namespace LoveSeat.Support
{
	public static class PlugExtensions
	{
		public static Plug With(this Plug aPlug, ViewOptions aViewOptions)
		{
			if (aViewOptions == null)
				return aPlug;

			if ((aViewOptions.Key != null) && (aViewOptions.Key.Count > 0))
				aPlug = aPlug.With(Constants.KEY, aViewOptions.Key.ToString());
			if ((aViewOptions.StartKey != null) && (aViewOptions.StartKey.HasValues))
				aPlug = aPlug.With(Constants.STARTKEY, aViewOptions.StartKey.ToString());
			if ((aViewOptions.EndKey != null) && (aViewOptions.EndKey.Count > 0))
				aPlug = aPlug.With(Constants.ENDKEY, aViewOptions.EndKey.ToString());
			if (aViewOptions.Limit.HasValue)
				aPlug = aPlug.With(Constants.LIMIT, aViewOptions.Limit.Value);
			if (aViewOptions.Skip.HasValue)
				aPlug = aPlug.With(Constants.SKIP, aViewOptions.Skip.ToString());
			if (aViewOptions.Reduce.HasValue)
				aPlug = aPlug.With(Constants.REDUCE, aViewOptions.Reduce.Value ? "true" : "false");
			if (aViewOptions.Group.HasValue)
				aPlug = aPlug.With(Constants.GROUP, aViewOptions.Group.Value);
			if (aViewOptions.InclusiveEnd.HasValue)
				aPlug = aPlug.With(Constants.INCLUSIVE_END, aViewOptions.InclusiveEnd.Value);
			if (aViewOptions.IncludeDocs.HasValue)
				aPlug = aPlug.With(Constants.INCLUDE_DOCS, aViewOptions.IncludeDocs.Value);
			if (aViewOptions.GroupLevel.HasValue)
				aPlug = aPlug.With(Constants.GROUP_LEVEL, aViewOptions.GroupLevel.Value);
			if (aViewOptions.Descending.HasValue)
				aPlug = aPlug.With(Constants.DESCENDING, aViewOptions.Descending.Value);
			if (aViewOptions.Stale.HasValue)
			{
				switch (aViewOptions.Stale.Value)
				{
					case Stale.Normal:
						aPlug = aPlug.With(Constants.STALE, Constants.OK);
						break;
					case Stale.UpdateAfter:
						aPlug = aPlug.With(Constants.STALE, Constants.UPDATE_AFTER);
						break;
					default:
						throw new ArgumentOutOfRangeException("Invalid Stale Option");
				}
			}
			if (!string.IsNullOrEmpty(aViewOptions.StartKeyDocId))
				aPlug = aPlug.With(Constants.STARTKEY_DOCID, aViewOptions.StartKeyDocId);
			if (!string.IsNullOrEmpty(aViewOptions.EndKeyDocId))
				aPlug = aPlug.With(Constants.ENDKEY_DOCID, aViewOptions.EndKeyDocId);
			if (!string.IsNullOrEmpty(aViewOptions.Etag))
				aPlug = aPlug.WithHeader(DreamHeaders.IF_NONE_MATCH, aViewOptions.Etag);
			return aPlug;
		}
		public static Plug With(this Plug aPlug, ChangeOptions aChangeOptions)
		{
			switch (aChangeOptions.Feed)
			{
				case ChangeFeed.LongPoll:
				case ChangeFeed.Normal:
					aPlug = aPlug.With(Constants.FEED, Constants.FEED_NORMAL);
					break;
				case ChangeFeed.Continuous:
					aPlug = aPlug.With(Constants.FEED, Constants.FEED_CONTINUOUS);
					break;
				default:
					//Never get here
					break;
			}

			if (!String.IsNullOrEmpty(aChangeOptions.Filter))
			{
				aPlug = aPlug.With(Constants.FILTER, XUri.Encode(aChangeOptions.Filter));
			}
			if (aChangeOptions.Heartbeat.HasValue)
			{
				aPlug = aPlug.With(Constants.HEARTBEAT, aChangeOptions.Heartbeat.Value);
			}
			if (aChangeOptions.IncludeDocs.HasValue)
			{
				aPlug = aPlug.With(Constants.INCLUDE_DOCS, aChangeOptions.IncludeDocs.Value ? "true" : "false");
			}
			if (aChangeOptions.Limit.HasValue)
			{
				aPlug = aPlug.With(Constants.LIMIT, aChangeOptions.Limit.Value);
			}
			if (aChangeOptions.Since.HasValue)
			{
				aPlug = aPlug.With(Constants.SINCE, aChangeOptions.Since.Value);
			}
			if (aChangeOptions.Timeout.HasValue)
			{
				aPlug = aPlug.With(Constants.TIMEOUT, aChangeOptions.Timeout.Value);
			}

			return aPlug;
		}
	}
}
