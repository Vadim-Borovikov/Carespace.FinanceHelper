using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CarespaceFinanceHelper.Dto.SelfWork
{
    public sealed class IncomeRequest
    {
        public sealed class Service
        {
            [JsonProperty]
            public string Name { get; set; }
            [JsonProperty]
            public decimal Amount { get; set; }
            [JsonProperty]
            public uint Quantity { get; set; }
        }

        public sealed class Client
        {
            [JsonProperty]
            public string ContactPhone { get; set; }
            [JsonProperty]
            public string DisplayName { get; set; }
            [JsonProperty]
            public string Inn { get; set; }
            [JsonProperty]
            public string IncomeType { get; set; }
        }

        [JsonProperty]
        public DateTime OperationTime { get; set; }
        [JsonProperty]
        public DateTime RequestTime { get; set; }
        [JsonProperty]
        public List<Service> Services { get; set; }
        [JsonProperty]
        public decimal TotalAmount { get; set; }
        [JsonProperty("client")]
        public Client ClientInfo { get; set; }
        [JsonProperty]
        public string PaymentType { get; set; }
        [JsonProperty]
        public bool IgnoreMaxTotalIncomeRestriction { get; set; }
    }
}
