﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoveSeat
{
	class Constants
	{
		public const string LOCALHOST = "localhost";
		public const int DEFAULT_PORT = 5984;

		public const string COMPACT_RUNNING = "compact_running";
		public const string DB_NAME = "db_name";
		public const string DISK_FORMAT_VERSION = "disk_format_version";
		public const string DISK_SIZE = "disk_size";
		public const string DOC_COUNT = "doc_count";
		public const string DOC_DEL_COUNT = "doc_del_count";
		public const string INSTANCE_STARTTIME = "instance_start_time";
		public const string PURGE_SEQUENCE = "purge_seq";
		public const string UPDATE_SEQUENCE = "update_seq";

		public const string STUB = "stub";
		public const string CONTENT_TYPE = "content_type";
		public const string LENGTH = "length";
		public const string DATA = "data";

		public const string REPLICATE = "_replicate";
		public const string CONFIG = "_config";

		public const string _REV = "_rev";
		public const string REV = "rev";

		public const string _ID = "_id";
		public const string ID = "id";

		public const string ALL_DOCS = "_all_docs";
		public const string DESIGN = "_design";
		public const string VIEW = "_view";

		public const string TYPE = "type";
		public const string NAME = "name";
		public const string ROLES = "roles";
		public const string TYPE_USER = "user";

		public const string ATTACHMENTS ="_attachments";
		public const string JAVASCRIPT = "javascript";
		public const string LANGUAGE = "language";
		public const string VIEWS = "views";

		public const string SOURCE = "source";
		public const string TARGET = "target";
		public const string CONTINUOUS = "continuous";
		public const string QUERY_PARAMS = "query_params";
		public const string CREATE_TARGET = "create_target";
		public const string FILTER = "filter";

		public const string KEY = "key";
		public const string STARTKEY = "startkey";
		public const string ENDKEY = "endkey";
		public const string LIMIT = "limit";
		public const string SKIP = "skip";
		public const string REDUCE = "reduce";
		public const string GROUP = "group";
		public const string INCLUDE_DOCS = "include_docs";
		public const string INCLUSIVE_END = "inclusive_end";
		public const string GROUP_LEVEL = "group_level";
		public const string DESCENDING = "descending";
		public const string STALE = "stale";
		public const string OK = "ok";
		public const string STARTKEY_DOCID = "startkey_docid";
		public const string ENDKEY_DOCID = "endkey_docid";

		public const string COMPACT = "_compact";
		public const string TEMP_VIEW = "_temp_view";
	}
}