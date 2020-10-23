using System;
using System.Collections.Generic;
using System.Linq;
using Carespace.FinanceHelper.Dto.Digiseller;
using Carespace.FinanceHelper.Dto.PayMaster;
using Carespace.FinanceHelper.Dto.SelfWork;
using Carespace.FinanceHelper.Providers;
using TokenResult = Carespace.FinanceHelper.Dto.Digiseller.TokenResult;

namespace Carespace.FinanceHelper
{
    public static class DataManager
    {
        #region Google

        public static IList<T> GetValues<T>(GoogleSheets provider, string range) where T : ILoadable, new()
        {
            IEnumerable<IList<object>> values = provider.GetValues(range, true);
            return values?.Select(LoadValues<T>).ToList();
        }

        public static void UpdateValues<T>(GoogleSheets provider, string range, IEnumerable<T> values)
            where T : ISavable
        {
            List<IList<object>> table = values.Select(v => v.Save()).ToList();
            provider.UpdateValues(range, table);
        }

        public static string ToString(this IList<object> values, int index)
        {
            return Extract(values, index, o => o?.ToString());
        }
        public static DateTime? ToDateTime(this IList<object> values, int index) => Extract(values, index, ToDateTime);
        internal static int? ToInt(this IList<object> values, int index) => Extract(values, index, ToInt);
        public static decimal? ToDecimal(this IList<object> values, int index) => Extract(values, index, ToDecimal);
        internal static Transaction.PayMethod? ToPayMathod(this IList<object> values, int index)
        {
            return Extract(values, index, ToPayMathod);
        }

        private static T Extract<T>(this IList<object> values, int index, Func<object, T> cast)
        {
            object o = values.Count > index ? values[index] : null;
            return cast(o);
        }

        private static DateTime? ToDateTime(object o) => o is long l ? (DateTime?) DateTime.FromOADate(l) : null;
        private static int? ToInt(object o) => int.TryParse(o?.ToString(), out int i) ? (int?) i : null;
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
        private static Transaction.PayMethod? ToPayMathod(object o)
        {
            return Enum.TryParse(o?.ToString(), out Transaction.PayMethod p) ? (Transaction.PayMethod?)p : null;
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

        public static void RegisterTaxes(IEnumerable<Transaction> transactions, string userAgent,
            string sourceDeviceId, string sourceType, string appVersion, string refreshToken, string incomeType,
            string paymentType, string nameFormat)
        {
            string token = null;
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (Transaction t in transactions)
            {
                if (!t.Price.HasValue || !string.IsNullOrWhiteSpace(t.TaxReceiptId))
                {
                    continue;
                }

                if (token == null)
                {
                    token = GetTaxToken(userAgent, sourceDeviceId, sourceType, appVersion, refreshToken);
                }

                var service = new IncomeRequest.Service
                {
                    Amount = t.Price.Value,
                    Name = GetTaxName(t, nameFormat),
                    Quantity = 1
                };
                var services = new List<IncomeRequest.Service> { service };

                IncomeResult result =
                    SelfWork.PostIncome(incomeType, t.Date, DateTime.Now, services, t.Price.Value, paymentType, token);

                t.TaxReceiptId = result.ApprovedReceiptUuid;
            }
        }

        public static string GetTaxToken(string userAgent, string sourceDeviceId, string sourceType,
            string appVersion, string refreshToken)
        {
            Dto.SelfWork.TokenResult result = SelfWork.GetToken(userAgent, sourceDeviceId, sourceType, appVersion, refreshToken);
            return result.Token;
        }

        #endregion // SelfWork

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Digiseller

        public static IEnumerable<Transaction> GetNewDigisellerSells(string login, string password, int sellerId,
            List<int> productIds, DateTime dateStart, DateTime dateFinish, string sellerSecret,
            IEnumerable<Transaction> oldTransactions)
        {
            IEnumerable<SellsResult.Sell> sells =
                GetDigisellerSells(sellerId, productIds, dateStart, dateFinish, sellerSecret);

            IEnumerable<int> oldSellIds = oldTransactions
                .Where(t => t.DigisellerSellId.HasValue)
                .Select(t => t.DigisellerSellId.Value);

            IEnumerable<SellsResult.Sell> newSells = sells.Where(s => !oldSellIds.Contains(s.InvoiceId));

            string token = GetToken(login, password, sellerSecret);

            foreach (SellsResult.Sell sell in newSells)
            {
                string promoCode = GetPromoCode(sell.InvoiceId, token);
                yield return CreateTransaction(sell, promoCode);
            }
        }

        private static IEnumerable<SellsResult.Sell> GetDigisellerSells(int sellerId, List<int> productIds,
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
                    yield return sell;
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

        private static Transaction CreateTransaction(SellsResult.Sell sell, string promoCode)
        {
            Transaction.PayMethod payMethod;
            switch (sell.PayMethodInfo)
            {
                case SellsResult.Sell.PayMethod.BankCard:
                    payMethod = Transaction.PayMethod.BankCard;
                    break;
                case SellsResult.Sell.PayMethod.Sbp:
                    payMethod = Transaction.PayMethod.Sbp;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new Transaction(sell.ProductName, sell.DatePay, sell.AmountIn, sell.InvoiceId, sell.ProductId, payMethod, promoCode);
        }

        private static string GetToken(string login, string password, string sellerSecret)
        {
            TokenResult result = Digiseller.GetToken(login, password, sellerSecret);
            return result.Token;
        }

        private static string GetPromoCode(int invoiceId, string token)
        {
            PurchaseResult result = Digiseller.GetPurchase(invoiceId, token);
            return result.Info.PromoCode;
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

        public static void CalculateShares(IEnumerable<Transaction> transactions, decimal taxFeePercent,
            decimal digisellerFeePercent, Dictionary<Transaction.PayMethod, decimal> payMasterFeePercents,
            Dictionary<string, List<Share>> shares)
        {
            var totals = new Dictionary<string, decimal>();
            foreach (Transaction transaction in transactions)
            {
                decimal amount = transaction.Amount;

                decimal? net = null;
                if (transaction.Price.HasValue)
                {
                    decimal price = transaction.Price.Value;

                    // Tax
                    decimal tax = Round(price * taxFeePercent);
                    transaction.Tax = tax;
                    amount -= transaction.Tax.Value;

                    net = amount;

                    if (transaction.DigisellerSellId.HasValue)
                    {
                        // Digiseller
                        decimal digisellerFee = Round(price * digisellerFeePercent);
                        transaction.DigisellerFee = digisellerFee;
                        amount -= digisellerFee;

                        // PayMaster
                        if (!transaction.PayMethodInfo.HasValue)
                        {
                            throw new ArgumentNullException();
                        }
                        decimal payMasterFee = Round(amount * payMasterFeePercents[transaction.PayMethodInfo.Value]);
                        transaction.PayMasterFee = payMasterFee;
                        amount -= payMasterFee;
                    }
                }

                string product = transaction.DigisellerProductId?.ToString() ?? NoProductSharesKey;
                foreach (Share share in shares[product])
                {
                    if (!transaction.Shares.ContainsKey(share.Agent))
                    {
                        transaction.Shares.Add(share.Agent, 0);
                    }

                    if (!totals.ContainsKey(share.Agent))
                    {
                        totals.Add(share.Agent, 0);
                    }

                    decimal value = Round(share.Calculate(amount, net, totals[share.Agent], transaction.PromoCode));

                    transaction.Shares[share.Agent] += value;
                    totals[share.Agent] += value;
                    amount -= value;
                }
            }
        }

        public static int? ExtractIntParameter(string value, string format)
        {
            string paramter = ExtractParameter(value, format);
            return int.TryParse(paramter, out int result) ? (int?) result : null;
        }

        public static string ExtractParameter(string value, string format)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            int left = format.IndexOf('{');
            int right = format.IndexOf('}');

            int prefixLength = left;
            int postfixLenght = format.Length - right - 1;

            return value.Substring(prefixLength, value.Length - prefixLength - postfixLenght);
        }

        internal static string GetHyperlink(string urlFormat, object parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter?.ToString()))
            {
                return null;
            }
            string url = string.Format(urlFormat, parameter);
            return string.Format(HyperlinkFormat, url, parameter);
        }

        private static decimal Round(decimal d) => Math.Round(d, 2);

        private const string HyperlinkFormat = "=HYPERLINK(\"{0}\";\"{1}\")";
        private const string NoProductSharesKey = "None";

        #endregion // Common
    }
}
