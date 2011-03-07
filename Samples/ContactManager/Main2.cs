using System;
using System.IO;
using LoveSeat.Interfaces;
using LoveSeat.Support;
using LoveSeat;
using MindTouch.Dream;
using MindTouch.Tasking;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace ContactManager
{

	internal class LogEntry
	{
		public string Details { get; set; }
		public DateTime LogDate { get; set; }
	}
	internal class ObjectSerializer<T>
	{
		private readonly JsonSerializerSettings theSettings;

		public ObjectSerializer ()
		{
			theSettings = new JsonSerializerSettings ();
			var converters = new List<JsonConverter> { new IsoDateTimeConverter () };
			theSettings.Converters = converters;
			theSettings.ContractResolver = new CamelCasePropertyNamesContractResolver ();
			theSettings.NullValueHandling = NullValueHandling.Ignore;
		}

		public virtual T Deserialize (string json)
		{
			return JsonConvert.DeserializeObject<T> (json, theSettings);
		}
		public virtual string Serialize (T obj)
		{
			return JsonConvert.SerializeObject (obj, Formatting.Indented, theSettings);
		}
	}
	class MainClass
	{
		public static void Main3 (string[] args)
		{
			
			
			LogEntry entry = new LogEntry { LogDate = new DateTime (2009, 2, 15, 0, 0, 0, DateTimeKind.Utc), Details = "Application started." };
			string isoJson = JsonConvert.SerializeObject (entry, new IsoDateTimeConverter ());
			
			Console.WriteLine (isoJson);
			
			LogEntry monLog = JsonConvert.DeserializeObject<LogEntry> (isoJson);
			Console.WriteLine (monLog.LogDate.ToString());
			// Tu peux tester comme ca, ca utilise  le même mechanisme pour sérialiser que LoveSeat
			// Je me demande si il n'ay a pas une version de cette librairie spéciale mono...
			
			
			
		}
	}
}

