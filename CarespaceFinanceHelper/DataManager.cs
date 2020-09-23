using System;
using System.Collections.Generic;
using System.Linq;
using CarespaceFinanceHelper.Dto.Digiseller;
using CarespaceFinanceHelper.Dto.SelfWork;

namespace CarespaceFinanceHelper
{
    public static class DataManager
    {
        public static IList<T> ReadValues<T>(GoogleSheetsProvider provider, string range) where T : ILoadable, new()
        {
            IEnumerable<IList<object>> values = provider.GetValues(range, true);
            return values?.Select(LoadValues<T>).ToList();
        }

        public static void WriteValues<T>(GoogleSheetsProvider provider, string range, IEnumerable<T> values,
            bool overwrite = false)
            where T : ISavable
        {
            List<IList<object>> table = values.Select(v => v.Save()).ToList();
            if (overwrite)
            {
                provider.UpdateValues(range, table);
            }
            else
            {
                provider.AppentValues(range, table);
            }
        }

        public static string GetTaxToken(string userAgent, string sourceDeviceId, string sourceType,
            string appVersion, string refreshToken)
        {
            TokenResult result =
                SelfWorkProvider.GetToken(userAgent, sourceDeviceId, sourceType, appVersion, refreshToken);
            return result.Token;
        }

        public static void RegisterTax(Transaction transaction, decimal amount, string incomeType,
            string paymentType, string taxNameFormat, string token)
        {
            var service = new IncomeRequest.Service
            {
                Amount = amount,
                Name = GetTaxName(transaction, taxNameFormat),
                Quantity = 1
            };
            var services = new List<IncomeRequest.Service> { service };

            IncomeResult result = SelfWorkProvider.PostIncome(incomeType, transaction.Date, DateTime.Now, services,
                amount, paymentType, token);

            transaction.TaxReceiptId = result.ApprovedReceiptUuid;
        }

        private static string GetTaxName(Transaction transaction, string taxNameFormat)
        {
            if (!transaction.DigisellerProductId.HasValue)
            {
                return transaction.Name;
            }

            ProductResult info = DigisellerProvider.GetProductsInfo(transaction.DigisellerProductId.Value);
            string productName = info.Info.Name;
            return string.Format(taxNameFormat, productName);
        }

        public static IEnumerable<Transaction> GetDigisellerSells(int sellerId, List<int> productIds,
            DateTime dateStat, DateTime dateFinish, string sellerSecret)
        {
            string start = dateStat.ToString(DateTimeFormat);
            string end = dateFinish.ToString(DateTimeFormat);
            int page = 1;
            int totalPages;
            do
            {
                SellsResult dto = DigisellerProvider.GetSells(sellerId, productIds, start, end, page, sellerSecret);
                foreach (SellsResult.Sell sell in dto.Sells)
                {
                    yield return CreateTransaction(sell);
                }
                ++page;
                totalPages = dto.Pages;
            } while (page <= totalPages);
        }

        public static string ToString(this IList<object> values, int index)
        {
            return Extract(values, index, o => o?.ToString());
        }
        public static DateTime? ToDateTime(this IList<object> values, int index) => Extract(values, index, ToDateTime);
        public static decimal? ToDecimal(this IList<object> values, int index) => Extract(values, index, ToDecimal);

        public static int? ExtractIntParameter(string value, string format)
        {
            string paramter = ExtractParameter(value, format);
            return int.TryParse(paramter, out int result) ? (int?) result : null;
        }

        public static string ExtractParameter(string value, string format)
        {
            if (value == null)
            {
                return null;
            }

            int left = format.IndexOf('{');
            int right = format.IndexOf('}');

            int prefixLength = left;
            int postfixLenght = format.Length - right - 1;

            return value.Substring(prefixLength, value.Length - prefixLength - postfixLenght);
        }

        internal static string Format(string format, object parameter)
        {
            return parameter == null ? null : string.Format(format, parameter);
        }

        private static Transaction CreateTransaction(SellsResult.Sell sell)
        {
            return new Transaction(sell.ProductName, sell.DatePay, sell.AmountIn, sell.InvoiceId, sell.ProductId);
        }

        private static T LoadValues<T>(IList<object> values) where T : ILoadable, new()
        {
            var instance = new T();
            instance.Load(values);
            return instance;
        }

        private static T Extract<T>(this IList<object> values, int index, Func<object, T> cast)
        {
            object o = values.Count > index ? values[index] : null;
            return cast(o);
        }

        private static DateTime? ToDateTime(object o) => o is long l ? (DateTime?) DateTime.FromOADate(l) : null;

        private static decimal? ToDecimal(object o)
        {
            switch (o)
            {
                case long l:
                    return l;
                case double d:
                    return (decimal) d;
                default:
                    return null;
            }
        }

        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    }
}
