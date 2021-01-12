using System.Collections.Generic;
using Newtonsoft.Json;

namespace Carespace.FinanceHelper.Console
{
    internal sealed class Configuration
    {
        [JsonProperty]
        public string GoogleSheetId { get; set; }

        [JsonProperty]
        public string GoogleCustomRange { get; set; }

        [JsonProperty]
        public string GoogleFinalRange { get; set; }


        [JsonProperty]
        public Dictionary<string, string> GoogleCredentials { get; set; }
        public string GoogleCredentialsJson => JsonConvert.SerializeObject(GoogleCredentials);


        [JsonProperty]
        public string DigisellerProductUrlFormat { get; set; }

        [JsonProperty]
        public string DigisellerSellUrlFormat { get; set; }

        [JsonProperty]
        public int DigisellerId { get; set; }

        [JsonProperty]
        public string DigisellerApiGuid { get; set; }

        [JsonProperty]
        public string DigisellerLogin { get; set; }

        [JsonProperty]
        public string DigisellerPassword { get; set; }


        [JsonProperty]
        public string TaxUserAgent { get; set; }

        [JsonProperty]
        public string TaxSourceDeviceId { get; set; }

        [JsonProperty]
        public string TaxSourceType { get; set; }

        [JsonProperty]
        public string TaxAppVersion { get; set; }

        [JsonProperty]
        public string TaxRefreshToken { get; set; }

        [JsonProperty]
        public string TaxIncomeType { get; set; }

        [JsonProperty]
        public string TaxPaymentType { get; set; }

        [JsonProperty]
        public string TaxProductNameFormat { get; set; }

        [JsonProperty]
        public string TaxReceiptUrlFormat { get; set; }


        [JsonProperty]
        public string PayMasterPaymentUrlFormat { get; set; }

        [JsonProperty]
        public string PayMasterLogin { get; set; }

        [JsonProperty]
        public string PayMasterPassword { get; set; }

        [JsonProperty]
        public List<string> PayMasterPurposesFormats { get; set; }


        [JsonProperty]
        public decimal TaxFeePercent { get; set; }

        [JsonProperty]
        public decimal DigisellerFeePercent { get; set; }

        [JsonProperty]
        public Dictionary<Transaction.PayMethod, decimal> PayMasterFeePercents { get; set; }


        [JsonProperty]
        public Dictionary<string, List<Share>> Shares { get; set; }
    }
}
