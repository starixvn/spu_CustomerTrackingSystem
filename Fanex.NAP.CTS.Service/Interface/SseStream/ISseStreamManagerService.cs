namespace Fanex.NAP.CTS.Service.Interface.SseStream
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    public interface ISseStreamManagerService
    {
        Task PushEventAsync<T>(T data, CancellationToken cancellationToken);

        ISseStreamManagerService Start(HttpContextBase httpContextBase);

        Task<ISseStreamManagerService> Process(Task[] tasks, CancellationToken cancellationToken);

        Task<ISseStreamManagerService> Complete(CancellationTokenSource cancellationTokenSource);
    }
}
