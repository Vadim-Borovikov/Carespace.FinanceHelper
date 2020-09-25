using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CarespaceFinanceHelper.Dto.PayMaster;
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
            Transaction.PayMasterPaymentUrlFormat = config.PayMasterPaymentUrlFormat;

            System.Console.Write("Loading google transactions... ");

            var transactions = new List<Transaction>();

            using (var provider = new GoogleSheets(config.GoogleCredentialsJson, config.GoogleSheetId))
            {
                IList<Transaction> oldTransactions =
                    DataManager.GetValues<Transaction>(provider, config.GoogleFinalRange);
                transactions.AddRange(oldTransactions);

                IList<Transaction> newCustomTransactions =
                    DataManager.GetValues<Transaction>(provider, config.GoogleCustomRange);
                if (newCustomTransactions != null)
                {
                    transactions.AddRange(newCustomTransactions);
                }

                System.Console.WriteLine("done.");

                System.Console.Write("Loading digiseller sells... ");

                DateTime dateStart = transactions.Select(o => o.Date).Min().AddDays(-1);
                DateTime dateEnd = DateTime.Today.AddDays(1);

                List<int> productIds = config.Shares.Keys.Where(k => k != "None").Select(int.Parse).ToList();
                IEnumerable<Transaction> newSells = DataManager.GetNewDigisellerSells(config.DigisellerId, productIds,
                    dateStart, dateEnd, config.DigisellerApiGuid, oldTransactions);
                transactions.AddRange(newSells);

                System.Console.WriteLine("done.");

                System.Console.Write("Calculating shares... ");

                DataManager.CalculateShares(transactions, config.TaxFeePercent, config.DigisellerFeePercent,
                    config.PayMasterFeePercents, config.Shares);

                System.Console.WriteLine("done.");

                List<Transaction> needPayment = transactions.Where(t => t.NeedPaynemt).ToList();
                if (needPayment.Any())
                {
                    System.Console.Write("Aquiring payments... ");

                    dateStart = needPayment.Select(o => o.Date).Min();
                    dateEnd = needPayment.Select(o => o.Date).Max().AddDays(1);
                    List<ListPaymentsFilterResult.Response.Payment> payments =
                        DataManager.GetPayments(dateStart, dateEnd, config.PayMasterLogin, config.PayMasterPassword);

                    foreach (Transaction transaction in needPayment)
                    {
                        DataManager.FindPayment(transaction, payments, config.PayMasterPurposesFormats);
                    }

                    System.Console.WriteLine("done.");
                }

                System.Console.Write("Register taxes... ");

                DataManager.RegisterTaxes(transactions, config.TaxUserAgent, config.TaxSourceDeviceId,
                    config.TaxSourceType, config.TaxAppVersion, config.TaxRefreshToken, config.TaxIncomeType,
                    config.TaxPaymentType, config.TaxProductNameFormat);

                System.Console.WriteLine("done.");

                System.Console.Write("Writing transactions to google... ");

                DataManager.UpdateValues(provider, config.GoogleFinalRange, transactions.OrderBy(t => t.Date));
                provider.ClearValues(config.GoogleCustomRange);
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
