using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            var transactions = new List<Transaction>();

            using (var provider = new GoogleSheetsProvider(config.GoogleCredentialsJson, config.GoogleSheetId))
            {
                IList<Transaction> customTransactions =
                    DataManager.ReadValues<Transaction>(provider, config.GoogleCustomRange);
                transactions.AddRange(customTransactions);

                System.Console.WriteLine("done.");

                System.Console.Write("Loading digiseller sells... ");

                List<Transaction> sells =
                    DataManager.GetDigisellerSells(config.DigisellerId, config.DigisellerProductIds,
                    config.EarliestDate, DateTime.Today, config.DigisellerSellUrlPrefix,
                    config.DigisellerProductUrlPrefix, config.DigisellerApiGuid).ToList();
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

                string name = GetTaxName(t, config);

                t.TaxReceiptUrl = DataManager.RegisterTax(name, t.Price.Value, t.Date, config.TaxIncomeType,
                    config.TaxPaymentType, token, config.TaxReceiptUrlFormat);
            }
        }

        private static string GetTaxName(Transaction transaction, Configuration config)
        {
            if (string.IsNullOrWhiteSpace(transaction.DigisellerProductUrl))
            {
                return transaction.Name;
            }

            string productName = DataManager.GetDigisellerProductName(transaction.DigisellerProductUrl,
                config.DigisellerProductUrlPrefix);
            return string.Format(config.TaxProductNameFormat, productName);
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
