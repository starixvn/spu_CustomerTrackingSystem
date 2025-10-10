using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Service.Interface.SseStream
{
    public interface ISseListenerService
    {
        Task ListenerAsync(
            string url,
            Action<string> onMessage,
            CancellationToken cancellationToken,
            int timeOutbySeconds = 120);
    }
}
