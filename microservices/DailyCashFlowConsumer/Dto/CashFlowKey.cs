using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DailyCashFlowConsumer.Dto
{
    public class CashFlowKey
    {
        [JsonPropertyName("USERID")]
        public string UserId { get; set; }

        [JsonPropertyName("DATE")]
        public DateOnly Date { get; set; }
    }
}