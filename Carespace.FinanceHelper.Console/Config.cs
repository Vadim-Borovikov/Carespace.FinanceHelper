using System.Collections.Generic;
using Newtonsoft.Json;

namespace Carespace.FinanceHelper.Console
{
    internal sealed class Config
    {
        [JsonProperty]
        public string GoogleSheetId { get; set; }

        [JsonProperty]
        public string GoogleSheetIdDonations { get; set; }

        [JsonProperty]
        public string GoogleDonationsRange { get; set; }

        [JsonProperty]
        public string GoogleDonationSumsRange { get; set; }

        [JsonProperty]
        public string GoogleCustomRange { get; set; }

        [JsonProperty]
        public string GoogleDonationsCustomRange { get; set; }

        [JsonProperty]
        public string GoogleDonationsCustomRangeToClear { get; set; }

        [JsonProperty]
        public string GoogleCustomRangeToClear { get; set; }

        [JsonProperty]
        public string GoogleFinalRange { get; set; }

        [JsonProperty]
        public Dictionary<string, string> GoogleCredential { get; set; }
        public string GoogleCredentialJson => JsonConvert.SerializeObject(GoogleCredential);

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
        public string TaxPaymentType { get; set; }

        [JsonProperty]
        public string TaxProductNameFormat { get; set; }

        [JsonProperty]
        public long TaxPayerId { get; set; }


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

        [JsonProperty]
        public int DonationsSiteId { get; set; }

        [JsonProperty]
        public string PaymasterSiteAliasDigiseller { get; set; }

        [JsonProperty]
        public string PaymasterSiteAliasDonations { get; set; }
    }
}
