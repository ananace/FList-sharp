using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace libflist.Util.Converters
{
	public sealed class JsonEnumConverter : JsonConverter
	{
		public enum EnumHandling
		{
			Default,

			Lowercase,
			Uppercase,
			Camelcase,

			Numeric
		}

		public EnumHandling Handling { get; set; }

		public JsonEnumConverter(EnumHandling type = EnumHandling.Default)
		{
			Handling = type;
		}

		public static T Convert<T>(string Token)
		{
			Type t = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
			var descs = t.GetMembers()
	             .Where(m => m.GetCustomAttribute<EnumValueAttribute>() != null)
	             .ToDictionary(m => m.GetCustomAttribute<EnumValueAttribute>().Name, m => m.Name);

			var enumText = Token;
			if (descs.ContainsKey(Token))
				enumText = descs[enumText];

			return (T)Enum.Parse(t, enumText, true);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}

			Type type = value.GetType();

			MemberInfo[] memberInfo = type.GetMember((value as Enum).ToString());
			if (memberInfo.Any())
			{
				var attrib = memberInfo.First().GetCustomAttribute<EnumValueAttribute>();

				if (attrib != null)
				{
					writer.WriteValue(attrib.Name);
					return;
				}
			}

			string enumName = (value as Enum).ToString("G");

			if (Handling == EnumHandling.Numeric || char.IsNumber(enumName[0]) || enumName[0] == '-')
			{
				// enum value has no name so write number
				writer.WriteValue(value);
			}
			else
			{
				string name = Enum.GetName(type, value);
				string finalName;

				switch (Handling)
				{
				case EnumHandling.Lowercase:
					finalName = name.ToLower(); break;
				case EnumHandling.Uppercase:
					finalName = name.ToUpper(); break;
				case EnumHandling.Camelcase:
					finalName = char.ToLower(name[0]) + name.Substring(1); break;
				default:
					finalName = name; break;
				}

				writer.WriteValue(finalName);
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				if (objectType.IsValueType || Nullable.GetUnderlyingType(objectType) == null)
					throw new JsonSerializationException("Can't deserialize null enums.");
				return null;
			}

			Type t = Nullable.GetUnderlyingType(objectType) ?? objectType;
			var descs = t.GetMembers()
				.Where(m => m.GetCustomAttribute<EnumValueAttribute>() != null)
				.ToDictionary(m => m.GetCustomAttribute<EnumValueAttribute>().Name, m => m.Name);

			try
			{
				if (reader.TokenType == JsonToken.String)
				{
					string enumText = reader.Value.ToString();

					if (descs.ContainsKey(enumText))
						enumText = descs[enumText];

					return Enum.Parse(t, enumText, true);
				}
				else if (reader.TokenType == JsonToken.Integer)
				{
					return Enum.Parse(objectType, reader.Value.ToString());
				}
			}
			catch (Exception ex)
			{
				throw new JsonSerializationException("Failed to convert value", ex);
			}

			throw new JsonSerializationException("Invalid enum data in JSON");
		}

		public override bool CanConvert(Type objectType)
		{
			return (Nullable.GetUnderlyingType(objectType) ?? objectType).IsEnum;
		}
	}
}
