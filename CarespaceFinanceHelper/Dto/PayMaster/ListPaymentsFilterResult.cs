using System.Collections.Generic;
using Newtonsoft.Json;

namespace CarespaceFinanceHelper.Dto.PayMaster
{
    public sealed class ListPaymentsFilterResult
    {
        public sealed class Response
        {
            public sealed class Payment
            {
                [JsonProperty]
                public int PaymentId { get; set; }

                [JsonProperty]
                public string Purpose { get; set; }
            }

            [JsonProperty]
            public List<Payment> Payments { get; set; }
        }

        [JsonProperty("response")]
        public Response ResponseInfo { get; set; }
    }
}
