using System.Collections.Generic;
using Newtonsoft.Json;

namespace CarespaceFinanceHelper.Tests
{
    internal class Configuration
    {
        public string GoogleSheetId { get; set; }

        public Dictionary<string, string> GoogleCredentials { get; set; }
        public string GoogleCredentialsJson => JsonConvert.SerializeObject(GoogleCredentials);
    }
}
