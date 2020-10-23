using System;
using System.Collections.Generic;

namespace Carespace.FinanceHelper.Tests
{
    internal sealed class Row : ISavable, ILoadable
    {
        public string Comment;

        public DateTime? Date;
        public decimal? Amount;

        public void Load(IList<object> values)
        {
            Comment = values.ToString(0);
            Date = values.ToDateTime(1);
            Amount = values.ToDecimal(2);
        }

        public IList<object> Save()
        {
            return new List<object>
            {
                Comment,
                $"{Date:dd MMMM yyyy}",
                $"{Amount}"
            };
        }
    }
}
