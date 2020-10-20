﻿using Newtonsoft.Json;

namespace CarespaceFinanceHelper.Dto.Digiseller
{
    internal sealed class PurchaseResult
    {
        public sealed class Content
        {
            [JsonProperty("promo_code")]
            public string PromoCode { get; set; }
        }

        [JsonProperty("content")]
        public Content Info { get; set; }
    }
}
