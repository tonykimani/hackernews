using Newtonsoft.Json;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace libs.Converters
{
    public class TimeConverter : JsonConverter
    {
        public TimeConverter()
        {
            
        }
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(int));
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is int)
            {
                var date = DateTimeOffset.FromUnixTimeSeconds((int)value);

                writer.WriteValue(date.ToString());

                return;
            }

            throw new JsonSerializationException($"Unexpected value when converting date.{value}");
        }
    }
}
