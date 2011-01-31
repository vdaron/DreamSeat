using System;
using System.Web;
using LoveSeat.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using MindTouch.Dream;

namespace LoveSeat
{


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