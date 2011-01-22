using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace LoveSeat
{
	public class CouchDatabaseInfo
	{
		[JsonProperty(Constants.COMPACT_RUNNING)]
		public bool CompactRunning { get; private set; }
		[JsonProperty(Constants.DB_NAME)]
		public string Name { get; private set; }
		[JsonProperty(Constants.DISK_FORMAT_VERSION)]
		public int DiskFormatVersion { get; private set; }
		[JsonProperty(Constants.DISK_SIZE)]
		public int DiskSize { get; private set; }
		[JsonProperty(Constants.DOC_COUNT)]
		public int DocCount { get; private set; }
		[JsonProperty(Constants.DOC_DEL_COUNT)]
		public int DocDeletedCount { get; private set; }
		[JsonProperty(Constants.INSTANCE_STARTTIME)]
		public string InstanceStartTime { get; private set; }
		[JsonProperty(Constants.PURGE_SEQUENCE)]
		public int PurgeSequence { get; private set; }
		[JsonProperty(Constants.UPDATE_SEQUENCE)]
		public int UpdateSequence { get; private set; } 
	}
}
