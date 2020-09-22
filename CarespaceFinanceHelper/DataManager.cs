using System;
using System.Collections.Generic;
using System.Linq;

namespace CarespaceFinanceHelper
{
    public static class DataManager
    {
        public static IList<T> GetValues<T>(GoogleSheetsProvider provider, string range) where T : ILoadable, new()
        {
            IEnumerable<IList<object>> values = provider.GetValues(range, true);
            return values?.Select(LoadValues<T>).ToList();
        }

        public static void AppendValues<T>(GoogleSheetsProvider provider, string range, IEnumerable<T> values,
            bool overwrite = false)
            where T : ISavable
        {
            List<IList<object>> table = values.Select(v => v.Save()).ToList();
            if (overwrite)
            {
                provider.UpdateValues(range, table);
            }
            else
            {
                provider.AppentValues(range, table);
            }
        }

        private static T LoadValues<T>(IList<object> values) where T : ILoadable, new()
        {
            var instance = new T();
            instance.Load(values);
            return instance;
        }

        public static DateTime ToDateTime(this object o) => DateTime.FromOADate((long)o);
        public static decimal ToDecimal(this object o) => decimal.Parse(o.ToString());
    }
}
