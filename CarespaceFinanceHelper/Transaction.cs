using System;
using System.Collections.Generic;

namespace CarespaceFinanceHelper
{
    public sealed class Transaction : ILoadable, ISavable
    {
        private string _name;
        private DateTime _date;
        private decimal _amount;
        private decimal? _price;
        private string _digisellerProductUrl;
        private string _taxReceiptUrl;

        public void Load(IList<object> values)
        {
            _name = values.ToString(0);

            DateTime? date = values.ToDateTime(1);
            if (!date.HasValue)
            {
                throw new ArgumentNullException($"Empty date in \"{_name}\"");
            }
            _date = date.Value;

            decimal? amount = values.ToDecimal(2);
            if (!amount.HasValue)
            {
                throw new ArgumentNullException($"Empty amount in \"{_name}\"");
            }
            _amount = amount.Value;

            _price = values.ToDecimal(3);
            _digisellerProductUrl = values.ToString(4);
            _taxReceiptUrl = values.ToString(5);
        }

        public IList<object> Save()
        {
            return new List<object>
            {
                _name,
                $"{_date:d MMMM yyyy}",
                $"{_amount}",
                $"{_price}",
                $"{_digisellerProductUrl}",
                $"{_taxReceiptUrl}"
            };
        }
    }
}
