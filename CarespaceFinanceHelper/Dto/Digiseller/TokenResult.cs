using Newtonsoft.Json;

namespace CarespaceFinanceHelper.Dto.Digiseller
{
    public sealed class TokenResult
    {
        [JsonProperty]
        public string Token { get; set; }
    }
}
