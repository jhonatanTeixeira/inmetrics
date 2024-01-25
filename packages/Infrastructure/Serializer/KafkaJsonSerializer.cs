using System.Text;
using System.Text.Json;
using Confluent.Kafka;

namespace Infrastructure.Serializer
{
    public class KafkaJsonSerializer<T> : ISerializer<T>, IDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new DateToTimestampConverter());
            options.Converters.Add(new DateOnlyTimeStampConverter());

            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data), options);
        }

        public byte[] Serialize(T data, SerializationContext context)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new DateToTimestampConverter());
            options.Converters.Add(new DateOnlyTimeStampConverter());

            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, options));
        }
    }
}