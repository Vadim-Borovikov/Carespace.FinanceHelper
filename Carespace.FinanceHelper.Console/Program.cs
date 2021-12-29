using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Carespace.FinanceHelper.Dto.PayMaster;
using GoogleSheetsManager;
using GoogleSheetsManager.Providers;
using Microsoft.Extensions.Configuration;

namespace Carespace.FinanceHelper.Console
{
    internal static class Program
    {
        private static void Main()
        {
            System.Console.Write("Reading config... ");

            Config config = GetConfig();

            System.Console.WriteLine("done.");

            Utils.PayMasterPaymentUrlFormat = config.PayMasterPaymentUrlFormat;

            Transaction.DigisellerSellUrlFormat = config.DigisellerSellUrlFormat;
            Transaction.DigisellerProductUrlFormat = config.DigisellerProductUrlFormat;

            Transaction.TaxPayerId = config.TaxPayerId;

            Transaction.Agents = config.Shares.Values
                                       .SelectMany(s => s)
                                       .Select(s => s.Agent)
                                       .Distinct()
                                       .OrderBy(s => s)
                                       .ToList();

            Transaction.Titles.AddRange(Transaction.Agents);

            System.Console.WriteLine("Updating purchases...");
            Task task = UpdatePurchasesAsync(config);
            task.Wait();
            System.Console.WriteLine("...done.");

            System.Console.WriteLine("Updating donations...");
            task = UpdateDonationsAsync(config);
            task.Wait();
            System.Console.WriteLine("...done.");
        }

        private static async Task UpdatePurchasesAsync(Config config)
        {
            using (var provider = new SheetsProvider(config.GoogleCredentialJson, ApplicationName, config.GoogleSheetId))
            {
                await LoadGoogleTransactionsAsync(provider, config);
            }
        }

        private static async Task UpdateDonationsAsync(Config config)
        {
            using (var provider =
                new SheetsProvider(config.GoogleCredentialJson, ApplicationName, config.GoogleSheetIdDonations))
            {
                await UpdateDonationsAsync(provider, config);
            }
        }

        private static async Task UpdateDonationsAsync(SheetsProvider provider, Config config)
        {
            var donations = new List<Donation>();

            System.Console.Write("> Loading google donations... ");

            IList<Donation> oldDonations = await DataManager.GetValuesAsync<Donation>(provider, config.GoogleDonationsRange);
            donations.AddRange(oldDonations);

            IList<Donation> newCustomDonations =
                await DataManager.GetValuesAsync<Donation>(provider, config.GoogleDonationsCustomRange);
            if (newCustomDonations != null)
            {
                donations.AddRange(newCustomDonations);
            }

            System.Console.WriteLine("done.");

            System.Console.Write("> Aquiring payments... ");

            DateTime dateStart = donations.Select(o => o.Date).Min().AddDays(-1);
            DateTime dateEnd = DateTime.Today.AddDays(1);

            List<Donation> newDonations = await Utils.GetNewPayMasterPaymentsAsync(config.PaymasterSiteAliasDonations,
                dateStart, dateEnd, config.PayMasterLogin, config.PayMasterPassword, oldDonations);

            donations.AddRange(newDonations);

            System.Console.WriteLine("done.");

            DateTime firstThursday = Utils.GetNextThursday(donations.Min(d => d.Date));

            Utils.CalculateTotalsAndWeeks(donations, config.PayMasterFeePercents, firstThursday);

            System.Console.Write("> Writing donations to google... ");

            await DataManager.UpdateValuesAsync(provider, config.GoogleDonationsRange,
                donations.OrderByDescending(d => d.Date).ToList());
            await provider.ClearValuesAsync(config.GoogleDonationsCustomRangeToClear);

            System.Console.WriteLine("done.");

            System.Console.Write("> Calculating and writing donations sums to google... ");

            List<DonationsSum> sums = donations.GroupBy(d => d.Week)
                                               .Select(g => new DonationsSum(firstThursday, g.Key, g.Sum(d => d.Total)))
                                               .ToList();

            await DataManager.UpdateValuesAsync(provider, config.GoogleDonationSumsRange,
                sums.OrderByDescending(s => s.Date).ToList());

            System.Console.WriteLine("done.");

        }

        private static async Task LoadGoogleTransactionsAsync(SheetsProvider provider, Config config)
        {
            var transactions = new List<Transaction>();

            System.Console.Write("> Loading google transactions... ");

            IList<Transaction> oldTransactions =
                await DataManager.GetValuesAsync<Transaction>(provider, config.GoogleFinalRange);
            transactions.AddRange(oldTransactions);

            IList<Transaction> newCustomTransactions =
                await DataManager.GetValuesAsync<Transaction>(provider, config.GoogleCustomRange);
            if (newCustomTransactions != null)
            {
                transactions.AddRange(newCustomTransactions);
            }

            System.Console.WriteLine("done.");

            System.Console.Write("> Loading digiseller sells... ");

            DateTime dateStart = transactions.Select(o => o.Date).Min().AddDays(-1);
            DateTime dateEnd = DateTime.Today.AddDays(1);

            List<int> productIds = config.Shares.Keys.Where(k => k != "None").Select(int.Parse).ToList();
            List<Transaction> newSells = await Utils.GetNewDigisellerSellsAsync(config.DigisellerLogin,
                config.DigisellerPassword, config.DigisellerId, productIds, dateStart, dateEnd, config.DigisellerApiGuid,
                oldTransactions);

            transactions.AddRange(newSells);

            System.Console.WriteLine("done.");

            System.Console.Write("> Calculating shares... ");

            Utils.CalculateShares(transactions, config.TaxFeePercent, config.DigisellerFeePercent,
                config.PayMasterFeePercents, config.Shares);

            System.Console.WriteLine("done.");

            List<Transaction> needPayment = transactions.Where(t => t.NeedPaynemt).ToList();
            if (needPayment.Any())
            {
                System.Console.Write("> Aquiring payments... ");

                dateStart = needPayment.Select(o => o.Date).Min();
                dateEnd = needPayment.Select(o => o.Date).Max().AddDays(1);
                List<ListPaymentsFilterResult.Response.Payment> payments =
                    await Utils.GetPaymentsAsync(config.PaymasterSiteAliasDigiseller, dateStart, dateEnd,
                        config.PayMasterLogin, config.PayMasterPassword);

                foreach (Transaction transaction in needPayment)
                {
                    Utils.FindPayment(transaction, payments, config.PayMasterPurposesFormats);
                }

                System.Console.WriteLine("done.");
            }

            System.Console.Write("> Register taxes... ");

            await Utils.RegisterTaxesAsync(transactions, config.TaxUserAgent, config.TaxSourceDeviceId, config.TaxSourceType,
                config.TaxAppVersion, config.TaxRefreshToken, config.TaxProductNameFormat);

            System.Console.WriteLine("done.");

            System.Console.Write("> Writing transactions to google... ");

            await DataManager.UpdateValuesAsync(provider, config.GoogleFinalRange, transactions.OrderBy(t => t.Date).ToList());
            await provider.ClearValuesAsync(config.GoogleCustomRangeToClear);

            System.Console.WriteLine("done.");
        }

        private static Config GetConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.override.json") // Create appsettings.override.json for private settings
                .Build()
                .Get<Config>();
        }

        private const string ApplicationName = "Carespace.FinanceHelper";
    }
}
