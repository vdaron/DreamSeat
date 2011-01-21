using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace LoveSeat
{
	internal class JsonDocumentConverter : JsonConverter
	{
		public override bool CanConvert(System.Type objectType)
		{
			return (objectType == typeof(JDocument)) ||(objectType == typeof(JObject));
		}

		public override object ReadJson(JsonReader reader, System.Type objectType, JsonSerializer serializer)
		{
			if(objectType == typeof(JDocument))
				return new JDocument(JObject.Load(reader));
			return JObject.Load(reader);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value is JObject)
				((JObject)value).WriteTo(writer);
			else
				((JDocument)value).WriteTo(writer);
		}
	}

	internal interface IObjectSerializer<T>
	{
		T Deserialize(string json);
		string Serialize(T obj);
	}

	internal class ObjectSerializer<T> : IObjectSerializer<T>
	{
		protected readonly JsonSerializerSettings settings;
		public ObjectSerializer()
		{
			settings = new JsonSerializerSettings();
			var converters = new List<JsonConverter> { new IsoDateTimeConverter(),new JsonDocumentConverter() };
			settings.Converters = converters;
			settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			settings.NullValueHandling = NullValueHandling.Ignore;
		}

		public virtual T Deserialize(string json)
		{
			return JsonConvert.DeserializeObject<T>(json, settings);
		}
		public virtual string Serialize(T obj)
		{
			return JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
		}
	}
}