using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace CarespaceFinanceHelper.Console
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            System.Console.Write("Reading config... ");

            Configuration config = GetConfig();
            System.Console.WriteLine("done.");
            System.Console.Write("Loading google transactions... ");

            using (var provider = new GoogleSheetsProvider(config.GoogleCredentialsJson, config.GoogleSheetId))
            {
                IList<Transaction> customTransactions =
                    DataManager.GetValues<Transaction>(provider, config.GoogleCustomRange);

                DataManager.AppendValues(provider, config.GoogleFinalRange, customTransactions, true);
            }

            System.Console.WriteLine("done.");
        }

        private static Configuration GetConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.override.json") // Create appsettings.override.json for private settings
                .Build()
                .Get<Configuration>();
        }
    }
}
