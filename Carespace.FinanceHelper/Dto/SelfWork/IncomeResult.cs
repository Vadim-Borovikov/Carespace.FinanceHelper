using Newtonsoft.Json;

namespace Carespace.FinanceHelper.Dto.SelfWork
{
    internal sealed class IncomeResult
    {
        [JsonProperty]
        public string ApprovedReceiptUuid { get; set; }
    }
}
