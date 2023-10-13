using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Serializer
{
    public class DateToTimestampConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            long unixTimestamp = reader.GetInt64();

            return DateTimeOffset.FromUnixTimeMilliseconds(unixTimestamp).DateTime;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            long unixTimestamp = new DateTimeOffset(value).ToUnixTimeMilliseconds();

            writer.WriteNumberValue(unixTimestamp);
        }
    }
}