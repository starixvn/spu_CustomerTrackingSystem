namespace Fanex.NAP.CTS.Service.Interface.SseStream
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ISseListenerService
    {
        Task ListenerAsync(
            string url,
            Action<string> onMessage,
            CancellationToken cancellationToken,
            int timeOutbySeconds = 120);
    }
}
