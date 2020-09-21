using System.Collections.Generic;

namespace CarespaceFinanceHelper
{
    public interface ISavable
    {
        IList<object> Save();
    }
}