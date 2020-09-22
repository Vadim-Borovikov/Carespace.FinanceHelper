using Newtonsoft.Json;

namespace CarespaceFinanceHelper.Dto.SelfWork
{
    internal sealed class TokenResult
    {
        [JsonProperty]
        public string Token { get; set; }
    }
}
