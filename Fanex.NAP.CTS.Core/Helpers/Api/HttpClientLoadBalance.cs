namespace Fanex.NAP.CTS.Core.Helpers.Api
{
    using Fanex.NAP.CTS.Core.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    public static class HttpClientLoadBalance
    {
        private static readonly object _lock = new object();
        private static int _nextIndex = 0;

        public static List<HttpClient> CreateClientBatch()
        {
            var count = ConfigSettings.AIInsightsHttpClientInstanceCount;
            var list = new List<HttpClient>(count);

            for (int i = 0; i < count; i++)
            {
                var client = new HttpClient
                {
                    Timeout = TimeSpan.FromHours(1.5), // 1.5 hours
                };
                list.Add(client);
            }

            return list;
        }

        public static HttpClient GetClientFrom(List<HttpClient> batch)
        {
            if (batch == null || batch.Count == 0)
                throw new InvalidOperationException("HttpClient batch is empty.");

            lock (_lock)
            {
                var client = batch[_nextIndex % batch.Count];
                _nextIndex++;
                return client;
            }
        }
    }
}
