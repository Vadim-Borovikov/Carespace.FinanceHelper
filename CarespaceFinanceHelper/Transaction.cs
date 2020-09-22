using System;
using System.Collections.Generic;

namespace CarespaceFinanceHelper
{
    public sealed class Transaction : ILoadable, ISavable
    {
        public string Name;
        public DateTime Date;
        private decimal _amount;
        public decimal? Price;
        public string DigisellerProductUrl;
        public string TaxReceiptUrl;

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
                $"{DigisellerProductUrl}",
                $"{TaxReceiptUrl}"
            };
        }
    }
}
