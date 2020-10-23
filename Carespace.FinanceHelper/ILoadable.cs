using System.Collections.Generic;

namespace Carespace.FinanceHelper
{
    public interface ILoadable
    {
        void Load(IList<object> values);
    }
}