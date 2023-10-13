using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DailyCashFlowConsumer.Dto
{
    public class CashFlowValue
    {
        [JsonPropertyName("TOTAL")]
        public decimal Total { get; set; }

    }
}