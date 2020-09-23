using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CarespaceFinanceHelper.Providers;
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

            Transaction.DigisellerSellUrlFormat = config.DigisellerSellUrlFormat;
            Transaction.DigisellerProductUrlFormat = config.DigisellerProductUrlFormat;
            Transaction.TaxReceiptUrlFormat = config.TaxReceiptUrlFormat;

            System.Console.Write("Loading google transactions... ");

            var transactions = new List<Transaction>();

            using (var provider = new GoogleSheets(config.GoogleCredentialsJson, config.GoogleSheetId))
            {
                IList<Transaction> customTransactions =
                    DataManager.ReadValues<Transaction>(provider, config.GoogleCustomRange);
                transactions.AddRange(customTransactions);

                System.Console.WriteLine("done.");

                System.Console.Write("Loading digiseller sells... ");

                List<Transaction> sells =
                    DataManager.GetDigisellerSells(config.DigisellerId, config.DigisellerProductIds,
                    config.EarliestDate, DateTime.Today, config.DigisellerApiGuid).ToList();
                transactions.AddRange(sells);

                System.Console.WriteLine("done.");

                System.Console.Write("Register taxes... ");

                RegisterTaxes(config, customTransactions);

                System.Console.WriteLine("done.");

                DataManager.WriteValues(provider, config.GoogleFinalRange, transactions.OrderBy(t => t.Date), true);
            }

            System.Console.WriteLine("done.");
        }

        private static void RegisterTaxes(Configuration config, IEnumerable<Transaction> transactions)
        {
            string token = null;
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (Transaction t in transactions)
            {
                if (!t.Price.HasValue || !string.IsNullOrWhiteSpace(t.TaxReceiptUrl))
                {
                    continue;
                }

                if (token == null)
                {
                    token = DataManager.GetTaxToken(config.TaxUserAgent, config.TaxSourceDeviceId,
                        config.TaxSourceType, config.TaxAppVersion, config.TaxRefreshToken);
                }

                DataManager.RegisterTax(t, t.Price.Value, config.TaxIncomeType, config.TaxPaymentType,
                    config.TaxProductNameFormat, token);
            }
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
