using System.Collections.Generic;
using Newtonsoft.Json;

namespace CarespaceFinanceHelper.Console
{
    internal class Configuration
    {
        public string GoogleSheetId { get; set; }

        public string GoogleCustomRange { get; set; }
        public string GoogleFinalRange { get; set; }

        public Dictionary<string, string> GoogleCredentials { get; set; }
        public string GoogleCredentialsJson => JsonConvert.SerializeObject(GoogleCredentials);
    }
}
