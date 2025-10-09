namespace Fanex.NAP.CTS.Service.Implement.SseStream
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Service.Interface.SseStream;
    using Newtonsoft.Json;
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using static Fanex.NAP.CTS.Service.Delegate.SseDelegate;

    public class SseSenderService : ISseSenderService
    {
        #region Constants
        private const int KEEP_ALIVE_INTERVAL_SECONDS = 15;
        private const string CONTENT_TYPE_EVENT_STREAM = "text/event-stream";
        private const string CACHE_CONTROL_NO_CACHE = "no-cache";
        private const string KEEP_ALIVE_MESSAGE = "event: keepalive\ndata: ping\n\n";
        #endregion

        #region Fields
        private HttpContextBase _httpContextStream;
        private Task _keepAliveTask;
        private Task _processingTask;
        #endregion

        #region Public Methods

        public ISseSenderService Start(HttpContextBase httpContextBase)
        {
            if (httpContextBase == null)
                throw new ArgumentNullException(nameof(httpContextBase));

            _httpContextStream = httpContextBase;

            ConfigureServerSentEvents(_httpContextStream);

            return this;
        }

        public async Task<ISseSenderService> Process(Func<PushEventDelegate, Task[]> taskFactories, CancellationTokenSource cancellationTokenSource)
        {
            if (taskFactories == null)
                throw new ArgumentNullException(nameof(taskFactories));

            if (cancellationTokenSource == null)
                throw new ArgumentNullException(nameof(cancellationTokenSource));

            PushEventDelegate pushDelegate = async (data) =>
            {
                await PushEventAsync(data, cancellationTokenSource.Token);
            };

            _keepAliveTask = Task.Run(async () => await MaintainConnectionAsync(cancellationTokenSource));

            var tasks = taskFactories(pushDelegate);
            _processingTask = Task.WhenAll(tasks);

            await Task.WhenAny(_keepAliveTask, _processingTask);
            cancellationTokenSource.Cancel();

            return this;
        }

        public async Task<ISseSenderService> Complete()
        {
            try
            {
                await Task.WhenAll(_keepAliveTask, _processingTask);
            }
            catch (OperationCanceledException)
            {
                // Normal cancellation behavior
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - SSE Stream Complete Error", ex);
            }
            finally
            {
                CloseResponse();
            }

            return this;
        }

        public async Task PushEventAsync<T>(T data, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!IsClientConnected())
                return;

            try
            {
                await SendEventToClientAsync(data, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Normal cancellation - no logging needed
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - SSE PushEvent Error", ex);
            }
        }

        #endregion

        #region Private Methods

        private void ConfigureServerSentEvents(HttpContextBase httpContext)
        {
            var response = httpContext.Response;

            // Set timeout to maximum
            httpContext.Server.ScriptTimeout = int.MaxValue;

            // Configure SSE headers
            response.ContentType = CONTENT_TYPE_EVENT_STREAM;
            response.CacheControl = CACHE_CONTROL_NO_CACHE;
            response.BufferOutput = false;
            response.Buffer = false;
            response.Charset = string.Empty;
        }

        private async Task MaintainConnectionAsync(CancellationTokenSource cancellationTokenSource)
        {
            var cancellationToken = cancellationTokenSource.Token;
            while (IsClientConnected() && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await SendKeepAliveAsync(cancellationToken);
                    await Task.Delay(TimeSpan.FromSeconds(KEEP_ALIVE_INTERVAL_SECONDS), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Normal cancellation - exit loop
                    break;
                }
                catch (Exception ex)
                {
                    LogExtension.LogError("CTS - SSE MaintainConnection Error", ex);
                    break;
                }
            }

            if (!IsClientConnected())
            {
                cancellationTokenSource.Cancel();
            }
        }

        private async Task SendKeepAliveAsync(CancellationToken cancellationToken)
        {
            await PushEventAsync(KEEP_ALIVE_MESSAGE, cancellationToken);
        }

        private async Task SendEventToClientAsync<T>(T data, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(data);
            var message = $"data: {json}\n\n";
            var buffer = Encoding.UTF8.GetBytes(message);

            await _httpContextStream.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
            await _httpContextStream.Response.OutputStream.FlushAsync(cancellationToken);
        }

        private bool IsClientConnected()
        {
            try
            {
                return _httpContextStream?.Response != null && _httpContextStream.Response.IsClientConnected;
            }
            catch
            {
                return false;
            }
        }

        private void CloseResponse()
        {
            try
            {
                _httpContextStream?.Response?.Close();
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - SSE Close Response Error", ex);
            }
        }

        #endregion
    }
}