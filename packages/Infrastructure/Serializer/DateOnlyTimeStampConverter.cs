using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Serializer
{
    public class DateOnlyTimeStampConverter: JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            long unixTimestamp = reader.GetInt64() * 24 * 60 * 60;

            return DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            long unixTimestamp = new DateTimeOffset(value.ToDateTime(TimeOnly.MinValue)).ToUnixTimeSeconds();

            writer.WriteNumberValue(unixTimestamp / 60 / 60 / 24);
        }
    }
}