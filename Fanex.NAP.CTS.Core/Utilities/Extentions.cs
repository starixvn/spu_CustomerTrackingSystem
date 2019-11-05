namespace Fanex.NAP.CTS.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Extentions
    {
        public static IEnumerable<IEnumerable<TValue>> Chunk2<TValue>(
            this IEnumerable<TValue> values,
            Int32 chunkSize)
        {
            return values
                .Select((v, i) => new { v, groupIndex = i / chunkSize })
                .GroupBy(x => x.groupIndex)
                .Select(g => g.Select(x => x.v));
        }

        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            var q = new Queue<T>();

            foreach (var item in source)
            {
                // check if q is full
                if(q.Count == chunksize)
                {
                    var ret = q;
                    q = new Queue<T>();
                    
                    yield return ret;
                }

                // enqueue item
                q.Enqueue(item);
            }

            yield return q;
        }
    }
}