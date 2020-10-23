using Newtonsoft.Json;

namespace Carespace.FinanceHelper.Dto.SelfWork
{
    internal sealed class TokenResult
    {
        [JsonProperty]
        public string Token { get; set; }
    }
}
