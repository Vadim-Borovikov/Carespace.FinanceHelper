using Newtonsoft.Json;

namespace CarespaceFinanceHelper.Dto.SelfWork
{
    internal sealed class IncomeResult
    {
        [JsonProperty]
        public string ApprovedReceiptUuid { get; set; }
    }
}
