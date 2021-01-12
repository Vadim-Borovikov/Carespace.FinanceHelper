using Newtonsoft.Json;

namespace Carespace.FinanceHelper.Tests
{
    internal sealed class Configuration
    {
        [JsonProperty]
        public string TaxSourceDeviceId { get; set; }

        [JsonProperty]
        public string TaxRefreshToken { get; set; }
    }
}
