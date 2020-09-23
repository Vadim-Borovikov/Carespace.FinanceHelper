﻿using System;
using System.Collections.Generic;
using System.Linq;
using CarespaceFinanceHelper.Dto.Digiseller;
using CarespaceFinanceHelper.Dto.PayMaster;
using CarespaceFinanceHelper.Dto.SelfWork;
using CarespaceFinanceHelper.Providers;

namespace CarespaceFinanceHelper
{
    public static class DataManager
    {
        #region Google

        public static IList<T> ReadValues<T>(GoogleSheets provider, string range) where T : ILoadable, new()
        {
            IEnumerable<IList<object>> values = provider.GetValues(range, true);
            return values?.Select(LoadValues<T>).ToList();
        }

        public static void WriteValues<T>(GoogleSheets provider, string range, IEnumerable<T> values,
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

        public static string ToString(this IList<object> values, int index)
        {
            return Extract(values, index, o => o?.ToString());
        }
        public static DateTime? ToDateTime(this IList<object> values, int index) => Extract(values, index, ToDateTime);
        public static decimal? ToDecimal(this IList<object> values, int index) => Extract(values, index, ToDecimal);

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

        private static T LoadValues<T>(IList<object> values) where T : ILoadable, new()
        {
            var instance = new T();
            instance.Load(values);
            return instance;
        }

        #endregion // Google

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region SelfWork

        public static string GetTaxToken(string userAgent, string sourceDeviceId, string sourceType,
            string appVersion, string refreshToken)
        {
            TokenResult result = SelfWork.GetToken(userAgent, sourceDeviceId, sourceType, appVersion, refreshToken);
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

            IncomeResult result =
                SelfWork.PostIncome(incomeType, transaction.Date, DateTime.Now, services, amount, paymentType, token);

            transaction.TaxReceiptId = result.ApprovedReceiptUuid;
        }

        #endregion // SelfWork

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Digiseller

        public static IEnumerable<Transaction> GetDigisellerSells(int sellerId, List<int> productIds,
            DateTime dateStat, DateTime dateFinish, string sellerSecret)
        {
            string start = dateStat.ToString(GoogleDateTimeFormat);
            string end = dateFinish.ToString(GoogleDateTimeFormat);
            int page = 1;
            int totalPages;
            do
            {
                SellsResult dto = Digiseller.GetSells(sellerId, productIds, start, end, page, sellerSecret);
                foreach (SellsResult.Sell sell in dto.Sells)
                {
                    yield return CreateTransaction(sell);
                }
                ++page;
                totalPages = dto.Pages;
            } while (page <= totalPages);
        }

        private static string GetTaxName(Transaction transaction, string taxNameFormat)
        {
            if (!transaction.DigisellerProductId.HasValue)
            {
                return transaction.Name;
            }

            ProductResult info = Digiseller.GetProductsInfo(transaction.DigisellerProductId.Value);
            string productName = info.Info.Name;
            return string.Format(taxNameFormat, productName);
        }

        private static Transaction CreateTransaction(SellsResult.Sell sell)
        {
            return new Transaction(sell.ProductName, sell.DatePay, sell.AmountIn, sell.InvoiceId, sell.ProductId);
        }

        private const string GoogleDateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        #endregion // Digiseller

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region PayMaster

        public static List<ListPaymentsFilterResult.Response.Payment> GetPayments(DateTime periodFrom,
            DateTime periodTo, string login, string password)
        {
            string start = periodFrom.ToString(PayMasterDateTimeFormat);
            string end = periodTo.ToString(PayMasterDateTimeFormat);

            ListPaymentsFilterResult result =
                PayMaster.GetPayments(login, password, "", "", start, end, "", PayMasterState);

            return result.ResponseInfo.Payments;
        }

        public static void FindPayment(Transaction transaction,
            IEnumerable<ListPaymentsFilterResult.Response.Payment> payments, IEnumerable<string> purposesFormats)
        {
            if (!transaction.DigisellerSellId.HasValue || transaction.PayMasterPaymentId.HasValue)
            {
                return;
            }

            IEnumerable<string> purposes =
                purposesFormats.Select(f => string.Format(f, transaction.DigisellerSellId.Value));

            ListPaymentsFilterResult.Response.Payment payment =
                payments.SingleOrDefault(p => purposes.Contains(p.Purpose));

            transaction.PayMasterPaymentId = payment?.PaymentId;
        }

        private const string PayMasterDateTimeFormat = "yyyy-MM-dd";
        private const string PayMasterState = "COMPLETE";

        #endregion // PayMaster

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Common

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

        #endregion // Common
    }
}