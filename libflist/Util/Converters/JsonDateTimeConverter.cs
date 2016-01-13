using System;
using Newtonsoft.Json;

namespace libflist.Util.Converters
{
	public sealed class JsonDateTimeConverter : JsonConverter
	{
		public enum TimestampPrecision
		{
			Seconds,
			Milliseconds,
			Microseconds,
			Nanoseconds
		}

		public TimestampPrecision Precision { get; set; }
		static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public JsonDateTimeConverter(TimestampPrecision prec = TimestampPrecision.Milliseconds)
		{
			Precision = prec;
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(DateTime);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.Integer)
				throw new JsonSerializationException("Can't deserialize datetime, wrong data in JSON");

			var t = (long)reader.Value;

			switch (Precision)
			{
			case TimestampPrecision.Seconds:
				return Epoch.AddSeconds(t);
			case TimestampPrecision.Milliseconds:
				return Epoch.AddMilliseconds(t);
			case TimestampPrecision.Microseconds:
				return Epoch.AddMilliseconds(t / 1000);
			case TimestampPrecision.Nanoseconds:
				return Epoch.AddMilliseconds(t / 1000000);
			}

			return null;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var timestamp = (DateTime)value - Epoch;

			switch (Precision)
			{
			case TimestampPrecision.Seconds:
				writer.WriteValue((long)timestamp.TotalSeconds); break;
			case TimestampPrecision.Milliseconds:
				writer.WriteValue((long)timestamp.TotalMilliseconds); break;
			case TimestampPrecision.Microseconds:
				writer.WriteValue((long)timestamp.TotalMilliseconds * 1000); break;
			case TimestampPrecision.Nanoseconds:
				writer.WriteValue((long)timestamp.TotalMilliseconds * 1000000); break;
			}
		}
	}
}