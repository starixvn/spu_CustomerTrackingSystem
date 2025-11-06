namespace Fanex.NAP.CTS.Service.Implement.SseStream
{
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Service.Interface.SseStream;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class SseListenerService : ISseListenerService
    {
        /// <summary>
        /// Connect to an SSE endpoint and stream messages continuously.
        /// </summary>
        public async Task ListenerAsync(
            string url,
            Action<string> onMessage,
            CancellationToken cancellationToken,
            int timeOutbySeconds = 120,
            int maxRetryCount = 3,
            int retryDelayMs = 2000)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("url cannot be null or empty.", nameof(url));
            if (onMessage == null)
                throw new ArgumentNullException(nameof(onMessage));

            await ApiWithHttpClientRotatorHelper
                .GetServerSentEventAsync(url, onMessage, timeOutbySeconds, cancellationToken, maxRetryCount, retryDelayMs)
                .ConfigureAwait(false);
        }
    }
}
