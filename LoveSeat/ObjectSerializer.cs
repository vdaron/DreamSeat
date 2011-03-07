using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace LoveSeat
{
	internal class JsonDocumentConverter : JsonConverter
	{
		public override bool CanConvert(System.Type objectType)
		{
			return (objectType == typeof(JDocument)) ||(objectType == typeof(JObject));
		}
		
		
		public override object ReadJson (JsonReader reader, System.Type objectType, JsonSerializer serializer)
		{
			return objectType == typeof(JDocument) ? new JDocument(JObject.Load(reader)) : JObject.Load(reader);
		}
			
		
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value is JObject)
			{
				((JObject)value).WriteTo(writer);
			}
			else
			{
				((JDocument)value).WriteTo(writer);
			}
		}
	}

	internal interface IObjectSerializer<T>
	{
		T Deserialize(string json);
		string Serialize(T obj);
	}

	internal class ObjectSerializer<T> : IObjectSerializer<T>
	{
		private readonly JsonSerializerSettings theSettings;

		public ObjectSerializer()
		{
			theSettings = new JsonSerializerSettings();
			var converters = new List<JsonConverter> { new IsoDateTimeConverter(), new JsonDocumentConverter()};
			theSettings.Converters = converters;
			theSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			theSettings.NullValueHandling = NullValueHandling.Ignore;
		}

		public virtual T Deserialize(string json)
		{
			return JsonConvert.DeserializeObject<T>(json, theSettings);
		}
		public virtual string Serialize(T obj)
		{
			return JsonConvert.SerializeObject(obj, Formatting.Indented, theSettings);
		}
	}
}