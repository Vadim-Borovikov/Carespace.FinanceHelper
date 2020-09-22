using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace CarespaceFinanceHelper.Tests
{
    internal static class Helper
    {
        public static Configuration GetConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json") // Create appsettings.json for private settings
                .Build()
                .Get<Configuration>();
        }
    }
}
