// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Carespace.FinanceHelper.Tests
{
    internal class Configuration
    {
        public string GoogleSheetId { get; set; }

        public string TaxSourceDeviceId { get; set; }
        public string TaxRefreshToken { get; set; }

        public Dictionary<string, string> GoogleCredentials { get; set; }
        public string GoogleCredentialsJson => JsonConvert.SerializeObject(GoogleCredentials);
    }
}
