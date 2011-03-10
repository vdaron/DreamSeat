using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;

namespace LoveSeat
{
	internal class JsonDocumentConverter : JsonConverter
	{
		public override bool CanConvert(System.Type objectType)
		{
			return (objectType == typeof(JDocument)) || (objectType.IsSubclassOf(typeof(JDocument)));
		}
		
		
		public override object ReadJson (JsonReader reader, System.Type objectType, JsonSerializer serializer)
		{

			return objectType == typeof(JDocument) ? new JDocument(JObject.Load(reader)) : Activator.CreateInstance(objectType,JObject.Load(reader));
		}
			
		
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
				((JDocument)value).WriteTo(writer);
		}
	}

	internal class JObjectConverter : JsonConverter
	{
		public override bool CanConvert(System.Type objectType)
		{
			return objectType == typeof(JObject);
		}
		public override object ReadJson(JsonReader reader, System.Type objectType, JsonSerializer serializer)
		{
			return JObject.Load(reader);
		}
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			((JObject)value).WriteTo(writer);
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
			var converters = new List<JsonConverter> { new IsoDateTimeConverter (), new JsonDocumentConverter (), new JObjectConverter () };
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
