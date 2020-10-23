using System.Collections.Generic;

namespace Carespace.FinanceHelper
{
    public interface ISavable
    {
        IList<object> Save();
    }
}