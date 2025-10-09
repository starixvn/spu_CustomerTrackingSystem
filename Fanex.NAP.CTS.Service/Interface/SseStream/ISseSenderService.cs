namespace Fanex.NAP.CTS.Service.Interface.SseStream
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using static Fanex.NAP.CTS.Service.Delegate.SseDelegate;

    public interface ISseSenderService
    {
        Task PushEventAsync<T>(T data, CancellationToken cancellationToken);

        ISseSenderService Start(HttpContextBase httpContextBase);

        Task<ISseSenderService> Process(Func<PushEventDelegate, Task[]> taskFactories, CancellationTokenSource cancellationTokenSource);

        Task<ISseSenderService> Complete();
    }
}
