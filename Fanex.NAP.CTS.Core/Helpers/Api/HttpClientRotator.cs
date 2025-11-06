namespace Fanex.NAP.CTS.Core.Helpers.Api
{
    using Fanex.NAP.CTS.Core.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;

    public static class HttpClientRotator
    {
        static HttpClientRotator()
        {
            // Global defaults
            ServicePointManager.DefaultConnectionLimit = 500;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.Expect100Continue = false;

            _activeClients = HttpClientLoadBalance.CreateClientBatch();
            _lastConfigHash = GetCurrentConfigHash();
        }

        private static readonly object _lock = new object();
        private static List<HttpClient> _activeClients;
        private static List<HttpClient> _standbyClients;
        private static List<HttpClient> _oldStandbyClients;

        private static DateTime _lastRotation = DateTime.UtcNow;
        private static string _lastConfigHash = "";

        public static HttpClient GetClient()
        {
            lock (_lock)
            {
                if (NeedRotation())
                {
                    RotateClients();
                }

                return HttpClientLoadBalance.GetClientFrom(_activeClients);
            }
        }

        private static bool NeedRotation()
        {
            if (DateTime.UtcNow - _lastRotation > GetRotationInterval())
                return true;

            var currentHash = GetCurrentConfigHash();
            return currentHash != _lastConfigHash;
        }

        private static TimeSpan GetRotationInterval()
        {
            return TimeSpan.FromHours(ConfigSettings.AIInsightsHttpTimeoutHours / 3);
        }

        private static void RotateClients()
        {
            // Dispose the oldest
            _oldStandbyClients?.ForEach(c => c.Dispose());

            // Rotate generations
            _oldStandbyClients = _standbyClients;
            _standbyClients = _activeClients;
            _activeClients = HttpClientLoadBalance.CreateClientBatch();

            _lastRotation = DateTime.UtcNow;
            _lastConfigHash = GetCurrentConfigHash();
        }

        private static string GetCurrentConfigHash()
        {
            var configString = $"{ConfigSettings.AIInsightsHttpTimeoutHours}|{ConfigSettings.AIInsightsHttpClientInstanceCount}";
            return configString.GetHashCode().ToString();
        }
    }
}
