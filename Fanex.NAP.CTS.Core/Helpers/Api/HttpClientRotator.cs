namespace Fanex.NAP.CTS.Core.Helpers.Api
{
    using System;
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
        }

        private static readonly object _lock = new object();
        private static HttpClient _activeClient;
        private static HttpClient _standbyClient;
        private static HttpClient _oldStandbyClient; // dispose this safely
        private static DateTime _lastRotation = DateTime.UtcNow;

        private static readonly TimeSpan RotationInterval = TimeSpan.FromMinutes(30);

        public static HttpClient GetClient()
        {
            lock (_lock)
            {
                if (DateTime.UtcNow - _lastRotation > RotationInterval)
                {
                    // Dispose the oldest client (lived RotationInterval x2 minutes)
                    _oldStandbyClient?.Dispose();

                    // Promote standby → oldStandby
                    _oldStandbyClient = _standbyClient;

                    // Promote active → standby
                    _standbyClient = _activeClient;

                    // Create a new active client
                    _activeClient = CreateHttpClient();

                    _lastRotation = DateTime.UtcNow;
                }

                _activeClient = _activeClient ?? CreateHttpClient();

                return _activeClient;
            }
        }

        private static HttpClient CreateHttpClient()
        {
            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromHours(1.5) // 1.5 hours
            };

            return client;
        }
    }
}
