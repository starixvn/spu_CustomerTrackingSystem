namespace Fanex.NAP.CTS.Core.Extentions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class IEnumerableExtension
    {
        public static string Join<T, TR>(this IEnumerable<T> data, Func<T, TR> selector, string seperator = ",")
        {
            return string.Join(seperator, data.Select(selector));
        }

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> data, Func<TSource, object> predicate)
        {
            return from item in data.GroupBy(predicate) select item.First();
        }

        public static bool IsNullOrEmpty<T> (this IEnumerable<T> arr)
        {
            return arr is null || !arr.Any();
        }

        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> arr)
        {
            return !IsNullOrEmpty(arr);
        }
    }
}
