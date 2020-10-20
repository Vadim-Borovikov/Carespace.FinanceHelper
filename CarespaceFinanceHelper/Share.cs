using System;
using Newtonsoft.Json;

namespace CarespaceFinanceHelper
{
    public sealed class Share
    {
        [JsonProperty]
        public string Agent { get; set; }

        [JsonProperty]
        public string Promo { get; set; }

        [JsonProperty]
        public decimal? Limit { get; set; }

        [JsonProperty]
        public decimal Value { get; set; }

        [JsonProperty]
        public decimal ValueAfterLimit { get; set; }

        internal decimal Calculate(decimal amount, decimal? net, decimal total, string promo)
        {
            if (!string.IsNullOrWhiteSpace(Promo))
            {
                if ((promo != Promo) || !net.HasValue)
                {
                    return 0;
                }
                amount = net.Value;
            }

            decimal value = amount * Value;

            if (!Limit.HasValue)
            {
                return value;
            }

            decimal beforeLimit = Math.Max(0, Limit.Value - total);
            if (beforeLimit == 0)
            {
                return amount * ValueAfterLimit;
            }

            if (value < 0)
            {
                return value;
            }

            decimal beforeLimitPart = Math.Min(value, beforeLimit);
            decimal afterLimitPart = ValueAfterLimit * Math.Max(0, value - beforeLimitPart);
            return beforeLimitPart + afterLimitPart;
        }
    }
}