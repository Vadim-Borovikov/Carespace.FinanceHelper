using System;
using System.Collections.Generic;
using System.Linq;

namespace Carespace.FinanceHelper
{
    public sealed class Transaction : ILoadable, ISavable
    {
        public enum PayMethod
        {
            BankCard,
            Sbp
        }

        // Common URL formats
        public static string DigisellerSellUrlFormat;
        public static string DigisellerProductUrlFormat;
        public static string TaxReceiptUrlFormat;
        public static string PayMasterPaymentUrlFormat;

        public static List<string> Agents;

        // Data
        internal string Name { get; private set; }
        public DateTime Date { get; private set; }
        internal decimal Amount { get; private set; }
        internal decimal? Price { get; private set; }
        internal string PromoCode { get; private set; }
        internal int? DigisellerSellId { get; private set; }
        internal int? DigisellerProductId { get; private set; }
        internal string TaxReceiptId;
        internal int? PayMasterPaymentId;
        internal PayMethod? PayMethodInfo { get; private set; }
        internal decimal? DigisellerFee;
        internal decimal? PayMasterFee;
        internal decimal? Tax;
        internal readonly Dictionary<string, decimal> Shares = new Dictionary<string, decimal>();

        public bool NeedPaynemt => DigisellerSellId.HasValue && !PayMasterPaymentId.HasValue;

        public Transaction() { }

        internal Transaction(string productName, DateTime datePay, decimal price, int digisellerSellId,
            int digisellerProductId, PayMethod payMethod, string promoCode)
        {
            Name = productName;
            Date = datePay;
            Amount = price;
            Price = price;
            DigisellerSellId = digisellerSellId;
            DigisellerProductId = digisellerProductId;
            PayMethodInfo = payMethod;
            PromoCode = promoCode;
        }

        public void Load(IList<object> values)
        {
            Name = values.ToString(0);

            DateTime? date = values.ToDateTime(1);
            if (!date.HasValue)
            {
                throw new ArgumentNullException($"Empty date in \"{Name}\"");
            }
            Date = date.Value;

            decimal? amount = values.ToDecimal(2);
            if (!amount.HasValue)
            {
                throw new ArgumentNullException($"Empty amount in \"{Name}\"");
            }
            Amount = amount.Value;

            Price = values.ToDecimal(3);

            PromoCode = values.ToString(4);

            DigisellerProductId = values.ToInt(5);

            PayMethodInfo = values.ToPayMathod(6);

            DigisellerSellId = values.ToInt(7);

            PayMasterPaymentId = values.ToInt(8);

            TaxReceiptId = values.ToString(9);
        }

        public IList<object> Save()
        {
            var result = new List<object>
            {
                Name,
                $"{Date:d MMMM yyyy}",
                $"{Amount}",
                $"{Price}",
                $"{PromoCode}",
                $"{DataManager.GetHyperlink(DigisellerProductUrlFormat, DigisellerProductId)}",
                $"{PayMethodInfo}",
                $"{DataManager.GetHyperlink(DigisellerSellUrlFormat, DigisellerSellId)}",
                $"{DataManager.GetHyperlink(PayMasterPaymentUrlFormat, PayMasterPaymentId)}",
                $"{DataManager.GetHyperlink(TaxReceiptUrlFormat, TaxReceiptId)}",
                $"{DigisellerFee}",
                $"{PayMasterFee}",
                $"{Tax}"
            };
            result.AddRange(Agents.Select(a => Shares.ContainsKey(a) ? $"{Shares[a]}" : ""));

            return result;
        }
    }
}
