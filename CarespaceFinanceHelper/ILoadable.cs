using System.Collections.Generic;

namespace CarespaceFinanceHelper
{
    public interface ILoadable
    {
        void Load(IList<object> values);
    }
}