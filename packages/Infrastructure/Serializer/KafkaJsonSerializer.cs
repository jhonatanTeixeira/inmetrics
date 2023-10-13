using System.Text;
using System.Text.Json;
using Confluent.Kafka;

namespace Infrastructure.Serializer
{
    public class KafkaJsonSerializer<T> : ISerializer<T>
    {
        public byte[] Serialize(T data, SerializationContext context)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new DateToTimestampConverter());

            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, options));
        }
    }
}