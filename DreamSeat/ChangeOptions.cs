namespace DreamSeat
{
	internal enum ChangeFeed
	{
		Normal,
		LongPoll, // unsupported
		Continuous
	}

	public class ChangeOptions
	{
		/// <summary>
		/// Include the document with the result
		/// </summary>
		internal bool? IncludeDocs { get; set; }

		/// <summary>
		/// Type of feed
		/// </summary>
		internal ChangeFeed Feed { get; set; }

		/// <summary>
		/// Filter function from a design document to get updates
		/// </summary>
		public string Filter { get; set; }
		
		/// <summary>
		/// Period (in milliseconds) after which an empty line is sent during longpoll or continuous
		/// </summary>
		public int? Heartbeat { get; set; }

		/// <summary>
		/// Maximum number of rows rows to return
		/// </summary>
		public int? Limit { get; set; }

		/// <summary>
		/// Start the results from the specified sequence number
		/// </summary>
		public int? Since { get; set; }

		/// <summary>
		/// Maximum period to wait before the response is sent
		/// </summary>
		public int? Timeout { get; set; }
	}
}
