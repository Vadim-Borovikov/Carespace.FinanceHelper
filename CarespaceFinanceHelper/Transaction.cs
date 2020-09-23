using System;
using System.Collections.Generic;

namespace CarespaceFinanceHelper
{
    public sealed class Transaction : ILoadable, ISavable
    {
        internal enum PayMethod
        {
            BankCard,
            Sbp
        }

        // Common URL formats
        public static string DigisellerSellUrlFormat;
        public static string DigisellerProductUrlFormat;
        public static string TaxReceiptUrlFormat;
        public static string PayMasterPaymentUrlFormat;

        // Data
        internal string Name { get; private set; }
        public DateTime Date { get; private set; }
        private decimal _amount;
        internal decimal? Price { get; private set; }
        internal int? DigisellerSellId { get; private set; }
        internal int? DigisellerProductId { get; private set; }
        internal string TaxReceiptId;
        internal int? PayMasterPaymentId;
        private PayMethod? _payMethod;

        public bool NeedPaynemt => DigisellerSellId.HasValue && !PayMasterPaymentId.HasValue;

        public Transaction() { }

        internal Transaction(string productName, DateTime datePay, decimal price, int digisellerSellId,
            int digisellerProductId, PayMethod payMethod)
        {
            Name = productName;
            Date = datePay;
            _amount = price;
            Price = price;
            DigisellerSellId = digisellerSellId;
            DigisellerProductId = digisellerProductId;
            _payMethod = payMethod;
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
            _amount = amount.Value;

            Price = values.ToDecimal(3);

            DigisellerProductId = values.ToInt(4);

            _payMethod = values.ToPayMathod(5);

            DigisellerSellId = values.ToInt(6);

            PayMasterPaymentId = values.ToInt(7);

            TaxReceiptId = values.ToString(8);
        }

        public IList<object> Save()
        {
            return new List<object>
            {
                Name,
                $"{Date:d MMMM yyyy}",
                $"{_amount}",
                $"{Price}",
                $"{DataManager.GetHyperlink(DigisellerProductUrlFormat, DigisellerProductId)}",
                $"{_payMethod}",
                $"{DataManager.GetHyperlink(DigisellerSellUrlFormat, DigisellerSellId)}",
                $"{DataManager.GetHyperlink(PayMasterPaymentUrlFormat, PayMasterPaymentId)}",
                $"{DataManager.GetHyperlink(TaxReceiptUrlFormat, TaxReceiptId)}"
            };
        }
    }
}
