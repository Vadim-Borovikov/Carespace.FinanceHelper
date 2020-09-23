using System;
using System.Collections.Generic;

namespace CarespaceFinanceHelper
{
    public sealed class Transaction : ILoadable, ISavable
    {
        internal string Name { get; private set; }
        public DateTime Date { get; private set; }
        private decimal _amount;
        public decimal? Price { get; private set; }

        private readonly int? _digisellerSellId;
        internal int? DigisellerProductId { get; private set; }
        internal string TaxReceiptId;

        private string DigisellerSellUrl => DataManager.Format(DigisellerSellUrlFormat, _digisellerSellId);

        private string DigisellerProductUrl
        {
            get => DataManager.Format(DigisellerProductUrlFormat, DigisellerProductId);
            set => DigisellerProductId = DataManager.ExtractIntParameter(value, DigisellerProductUrlFormat);
        }

        public string TaxReceiptUrl
        {
            get => DataManager.Format(TaxReceiptUrlFormat, TaxReceiptId);
            private set => TaxReceiptId = DataManager.ExtractParameter(value, TaxReceiptUrlFormat);
        }

        public static string DigisellerSellUrlFormat;
        public static string DigisellerProductUrlFormat;
        public static string TaxReceiptUrlFormat;

        public Transaction() { }

        internal Transaction(string productName, DateTime datePay, decimal price, int invoiceId, int productId)
        {
            Name = productName;
            Date = datePay;
            _amount = price;
            Price = price;
            _digisellerSellId = invoiceId;
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
            TaxReceiptUrl = values.ToString(5);
        }

        public IList<object> Save()
        {
            return new List<object>
            {
                Name,
                $"{Date:d MMMM yyyy}",
                $"{_amount}",
                $"{Price}",
                $"{DigisellerSellUrl}",
                $"{DigisellerProductUrl}",
                $"{TaxReceiptUrl}"
            };
        }
    }
}
