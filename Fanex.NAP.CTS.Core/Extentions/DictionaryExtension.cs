using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.CTS.Core.Extentions
{
    public static class DictionaryExtension
    {
        public static Dictionary<TKey, TValue> Distinct<TKey, TValue, TItem>(
            this IEnumerable<TItem> arr,
            Func<TItem, TKey> selectorKey,
            Func<TItem, TValue> selectorValue)
        {
            return arr.Aggregate(new Dictionary<TKey, TValue>(), (list, item) =>
            {
                var key = selectorKey.Invoke(item);
                if (!list.ContainsKey(key))
                {
                    list.Add(key, selectorValue.Invoke(item));
                }

                return list;
            });
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            var val = default(TValue);

            dictionary?.TryGetValue(key, out val);

            return val;
        }
    }
}
