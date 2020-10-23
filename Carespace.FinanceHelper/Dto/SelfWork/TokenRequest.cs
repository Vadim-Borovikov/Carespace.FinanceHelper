using Newtonsoft.Json;

namespace Carespace.FinanceHelper.Dto.SelfWork
{
    internal sealed class TokenRequest
    {
        public sealed class Device
        {
            public sealed class MetaDetails
            {
                [JsonProperty]
                public string UserAgent { get; set; }
            }

            [JsonProperty]
            public string SourceDeviceId { get; set; }
            [JsonProperty]
            public string SourceType { get; set; }
            [JsonProperty]
            public string AppVersion { get; set; }
            [JsonProperty("metaDetails")]
            public MetaDetails MetaDetailsInfo { get; set; }
        }

        [JsonProperty]
        public Device DeviceInfo { get; set; }
        [JsonProperty]
        public string RefreshToken { get; set; }
    }
}
