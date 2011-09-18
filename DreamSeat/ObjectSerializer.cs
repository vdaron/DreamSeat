using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;

namespace DreamSeat
{
	internal class JsonDocumentConverter : JsonConverter
	{
		public override bool CanConvert(Type aType)
		{
			if (aType == null)
				throw new ArgumentNullException("aType");

			return (aType == typeof(JDocument)) || (aType.IsSubclassOf(typeof(JDocument)));
		}
		public override object ReadJson (JsonReader aReader, Type aType, JsonSerializer aSerializer)
		{
			if (aReader == null)
				throw new ArgumentNullException("aReader");
			if (aType == null)
				throw new ArgumentNullException("aType");

			return aType == typeof(JDocument) ? new JDocument(JObject.Load(aReader)) : Activator.CreateInstance(aType,JObject.Load(aReader));
		}
		public override void WriteJson(JsonWriter aWriter, object aValue, JsonSerializer aSerializer)
		{
			if (aWriter == null)
				throw new ArgumentNullException("aWriter");
			if (aValue == null)
				throw new ArgumentNullException("aValue");

			((JDocument)aValue).WriteTo(aWriter);
		}
	}

	internal class JObjectConverter : JsonConverter
	{
		public override bool CanConvert(Type aType)
		{
			return aType == typeof(JObject);
		}
		public override object ReadJson(JsonReader aReader, Type aType, JsonSerializer aSerializer)
		{
			if (aReader == null)
				throw new ArgumentNullException("aReader");
			if (aType == null)
				throw new ArgumentNullException("aType");
			return JObject.Load(aReader);
		}

		public override void WriteJson(JsonWriter aWriter, object aValue, JsonSerializer aSerializer)
		{
			if (aWriter == null)
				throw new ArgumentNullException("aWriter");
			if (aValue == null)
				throw new ArgumentNullException("aValue");
			((JObject)aValue).WriteTo(aWriter);
		}
	}

	internal interface IObjectSerializer<T>
	{
		T Deserialize(string aJson);
		string Serialize(T anObj);
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

		public virtual T Deserialize(string aJson)
		{
			return JsonConvert.DeserializeObject<T>(aJson, theSettings);
		}
		public virtual string Serialize(T anObj)
		{
			return JsonConvert.SerializeObject(anObj, Formatting.Indented, theSettings);
		}
	}
}
