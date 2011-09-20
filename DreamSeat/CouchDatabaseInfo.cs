using Newtonsoft.Json;
using System;

namespace DreamSeat
{
	public class CouchDatabaseInfo
	{
		private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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
		public double InstanceStartTimeMs { get; private set; }
		[JsonProperty(Constants.PURGE_SEQUENCE)]
		public int PurgeSequence { get; private set; }
		[JsonProperty(Constants.UPDATE_SEQUENCE)]
		public int UpdateSequence { get; private set; }

		public DateTime InstanceStartTime { get { return Epoch.AddMilliseconds(InstanceStartTimeMs/1000); } }
	}
}
