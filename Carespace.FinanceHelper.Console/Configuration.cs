// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CollectionNeverUpdated.Global

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Carespace.FinanceHelper.Console
{
    internal class Configuration
    {
        public string GoogleSheetId { get; set; }
        public string GoogleCustomRange { get; set; }
        public string GoogleFinalRange { get; set; }

        public Dictionary<string, string> GoogleCredentials { get; set; }
        public string GoogleCredentialsJson => JsonConvert.SerializeObject(GoogleCredentials);

        public string DigisellerProductUrlFormat { get; set; }
        public string DigisellerSellUrlFormat { get; set; }
        public int DigisellerId { get; set; }
        public string DigisellerApiGuid { get; set; }
        public string DigisellerLogin { get; set; }
        public string DigisellerPassword { get; set; }

        public string TaxUserAgent { get; set; }
        public string TaxSourceDeviceId { get; set; }
        public string TaxSourceType { get; set; }
        public string TaxAppVersion { get; set; }
        public string TaxRefreshToken { get; set; }
        public string TaxIncomeType { get; set; }
        public string TaxPaymentType { get; set; }
        public string TaxProductNameFormat { get; set; }
        public string TaxReceiptUrlFormat { get; set; }

        public string PayMasterPaymentUrlFormat { get; set; }
        public string PayMasterLogin { get; set; }
        public string PayMasterPassword { get; set; }
        public List<string> PayMasterPurposesFormats { get; set; }

        public decimal TaxFeePercent { get; set; }
        public decimal DigisellerFeePercent { get; set; }
        public Dictionary<Transaction.PayMethod, decimal> PayMasterFeePercents { get; set; }

        public Dictionary<string, List<Share>> Shares { get; set; }
    }
}
