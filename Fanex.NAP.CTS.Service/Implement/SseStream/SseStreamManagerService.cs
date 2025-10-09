namespace Fanex.NAP.CTS.Service.Implement.SseStream
{
    using Fanex.NAP.CTS.Service.Interface.SseStream;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using static Fanex.NAP.CTS.Service.Delegate.SseDelegate;

    public class SseStreamManagerService : ISseStreamManagerService
    {
        private readonly ISseListenerService _listenerService;
        private readonly ISseSenderService _senderService;

        public SseStreamManagerService()
        {
            _listenerService = new SseListenerService();
            _senderService = new SseSenderService();
        }

        public async Task ListenerAsync(string url, Action<string> onMessage, int timeOutbySeconds = 120, CancellationToken? cancellationToken = null)
        {
            await _listenerService.ListenerAsync(url, onMessage, timeOutbySeconds, cancellationToken);
        }

        public async Task SenderAsync(HttpContextBase httpContext, CancellationToken cancellationToken, Func<PushEventDelegate, Task[]> taskFactories)
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _senderService.Start(httpContext);

            await _senderService.Process(taskFactories, linkedCts);

            await _senderService.Complete();
        }
    }
}
