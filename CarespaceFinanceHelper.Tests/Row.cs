using System;
using System.Collections.Generic;

namespace CarespaceFinanceHelper.Tests
{
    internal sealed class Row : ISavable, ILoadable
    {
        public string Comment;

        public DateTime? Date;
        public decimal? Amount;

        public void Load(IList<object> values)
        {
            Comment = values[0].ToString();
            Date = values[1].ToDateTime();
            Amount = values[2].ToDecimal();
        }

        public IList<object> Save()
        {
            return new List<object>
            {
                Comment,
                $"{Date:dd MMMM yyyy}",
                $"{Amount:C}"
            };
        }
    }
}
