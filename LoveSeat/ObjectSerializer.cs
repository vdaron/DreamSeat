using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace LoveSeat
{
	public interface IObjectSerializer<T>
	{
		T Deserialize(string json);
		string Serialize(T obj);
	}

	public class ObjectSerializer<T> : IObjectSerializer<T>
	{
		protected readonly JsonSerializerSettings settings;
		public ObjectSerializer()
		{
			settings = new JsonSerializerSettings();
			var converters = new List<JsonConverter> { new IsoDateTimeConverter() };
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

	public class JObjectSerializer : IObjectSerializer<JsonDocument>
	{
		public JsonDocument Deserialize(string json)
		{
			return new JsonDocument(JObject.Parse(json));
		}

		public string Serialize(JsonDocument obj)
		{
			return obj.ToString();
		}
	}
}