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

        public static string ToString(this IList<object> values, int index)
        {
            return Extract(values, index, o => o?.ToString());
        }
        public static DateTime? ToDateTime(this IList<object> values, int index) => Extract(values, index, ToDateTime);
        public static decimal? ToDecimal(this IList<object> values, int index) => Extract(values, index, ToDecimal);

        private static T Extract<T>(this IList<object> values, int index, Func<object, T> cast)
        {
            object o = values.Count > index ? values[index] : null;
            return cast(o);
        }

        private static DateTime? ToDateTime(object o) => o is long l ? (DateTime?) DateTime.FromOADate(l) : null;

        private static decimal? ToDecimal(object o)
        {
            switch (o)
            {
                case long l:
                    return l;
                case double d:
                    return (decimal) d;
                default:
                    return null;
            }
        }
    }
}
