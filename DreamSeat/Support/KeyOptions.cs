using System;
using System.Text;
using DreamSeat.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace DreamSeat.Support
{
	public class KeyOptions : JArray
	{
		public KeyOptions(params object[] aValues)
			:base(aValues)
		{
		}

		public override string ToString()
		{
			if (Count == 0)
				return String.Empty;
			if (Count == 1)
				return this[0].ToString(Formatting.None, new IsoDateTimeConverter());

			StringBuilder result = new StringBuilder("[");
			bool first = true;
			foreach (var item in this)
			{
				if (!first)
					result.Append(',');
				first = false;
				result.Append(item.ToString(Formatting.None, new IsoDateTimeConverter()));
			}
			result.Append(']');
			return result.ToString();
		}
	}
}
