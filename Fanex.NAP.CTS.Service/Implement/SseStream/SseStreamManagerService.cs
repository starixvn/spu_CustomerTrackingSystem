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

        public SseStreamManagerService(ISseListenerService listener, ISseSenderService sender)
        {
            _listenerService = listener ?? new SseListenerService();
            _senderService = sender ?? new SseSenderService();
        }

        public async Task ListenerAsync(string url, Action<string> onMessage, CancellationToken cancellationToken, int timeOutbySeconds = 120)
        {
            await _listenerService
                .ListenerAsync(url, onMessage, cancellationToken, timeOutbySeconds)
                .ConfigureAwait(false);
        }

        public async Task SenderAsync(HttpContextBase httpContext, CancellationToken cancellationToken, Func<PushEventDelegate, Task[]> taskFactories)
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _senderService.Start(httpContext);

            await _senderService.Process(taskFactories, linkedCts).ConfigureAwait(false);

            await _senderService.Complete().ConfigureAwait(false);
        }
    }
}
