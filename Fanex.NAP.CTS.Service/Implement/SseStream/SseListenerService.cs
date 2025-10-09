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
            int timeOutbySeconds = 120,
            CancellationToken? cancellationToken = null)
        {
            await ApiWithHttpClientRotatorHelper.GetServerSentEventAsync(url, onMessage, timeOutbySeconds, cancellationToken);
        }
    }
}
