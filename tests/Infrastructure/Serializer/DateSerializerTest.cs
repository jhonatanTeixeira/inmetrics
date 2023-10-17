using System.Text.Json;
using Infrastructure.Serializer;
using Xunit;

namespace tests.Infrastructure.Serializer
{
    public class DateSerializerTest
    {
        [Fact]
        public void CanConvertDateOnlyToJsonAndBack()
        {
            var converter = new DateOnlyTimeStampConverter();
            var date = new DateOnly(2023, 10, 17);

            var options = new JsonSerializerOptions
            {
                Converters = { converter },
                PropertyNameCaseInsensitive = true
            };

            string json = JsonSerializer.Serialize(date, options);
            var deserializedDate = JsonSerializer.Deserialize<DateOnly>(json, options);

            Assert.Equal(date, deserializedDate);

            long expectedTimestamp = 19647;
            var jsonElement = JsonDocument.Parse(json).RootElement;
            long actualTimestamp = jsonElement.GetInt64();

            Assert.Equal(expectedTimestamp, actualTimestamp);
        }
    }
}