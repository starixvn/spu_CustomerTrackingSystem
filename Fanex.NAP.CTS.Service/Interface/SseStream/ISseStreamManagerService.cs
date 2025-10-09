namespace Fanex.NAP.CTS.Service.Interface.SseStream
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using static Fanex.NAP.CTS.Service.Delegate.SseDelegate;

    public interface ISseStreamManagerService
    {
        Task SenderAsync(
            HttpContextBase httpContext,
            CancellationToken cancellationToken,
            Func<PushEventDelegate, Task[]> taskFactories);

        Task ListenerAsync(
            string url,
            Action<string> onMessage,
            int timeOutbySeconds = 120,
            CancellationToken? cancellationToken = null);
    }
}
