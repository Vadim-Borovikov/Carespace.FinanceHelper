using System;
using System.Collections.Generic;

namespace CarespaceFinanceHelper
{
    public sealed class Transaction : ILoadable, ISavable
    {
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

        // URL readers-writers
        private string DigisellerSellUrl
        {
            get => DataManager.Format(DigisellerSellUrlFormat, DigisellerSellId);
            set => DigisellerSellId = DataManager.ExtractIntParameter(value, DigisellerSellUrlFormat);
        }
        private string DigisellerProductUrl
        {
            get => DataManager.Format(DigisellerProductUrlFormat, DigisellerProductId);
            set => DigisellerProductId = DataManager.ExtractIntParameter(value, DigisellerProductUrlFormat);
        }
        private string TaxReceiptUrl
        {
            get => DataManager.Format(TaxReceiptUrlFormat, TaxReceiptId);
            set => TaxReceiptId = DataManager.ExtractParameter(value, TaxReceiptUrlFormat);
        }
        private string PayMasterPaymentUrl
        {
            get => DataManager.Format(PayMasterPaymentUrlFormat, PayMasterPaymentId);
            set => PayMasterPaymentId = DataManager.ExtractIntParameter(value, PayMasterPaymentUrlFormat);
        }

        public Transaction() { }

        internal Transaction(string productName, DateTime datePay, decimal price, int invoiceId, int productId)
        {
            Name = productName;
            Date = datePay;
            _amount = price;
            Price = price;
            DigisellerSellId = invoiceId;
            DigisellerProductId = productId;
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

            DigisellerProductUrl = values.ToString(4);

            DigisellerSellUrl = values.ToString(5);

            PayMasterPaymentUrl = values.ToString(6);

            TaxReceiptUrl = values.ToString(7);
        }

        public IList<object> Save()
        {
            return new List<object>
            {
                Name,
                $"{Date:d MMMM yyyy}",
                $"{_amount}",
                $"{Price}",
                $"{DigisellerProductUrl}",
                $"{DigisellerSellUrl}",
                $"{PayMasterPaymentUrl}",
                $"{TaxReceiptUrl}"
            };
        }
    }
}
