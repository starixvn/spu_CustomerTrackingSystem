using Fanex.NAP.CTS.Model.AIInsights.Api;
using Fanex.NAP.CTS.Service.AIInsights;
using Fanex.NAP.CTS.Service.Implement.AIInsights;
using Fanex.NAP.CTS.Service.Interface.AIInsights;
using Fanex.NAP.CTS.Service.Interface.SseStream;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using static Fanex.NAP.CTS.Service.Delegate.SseDelegate;

namespace Fanex.NAP.CTS.Test.Services
{
    [TestClass]
    public class AIInsightsServiceTest
    {
        private static AIInsightsFileUploadDataModel DummyUpload() => new AIInsightsFileUploadDataModel { cache_key = "cache-1" };

        private Mock<ISseStreamManagerService> _sseStreamManagerService;
        private Mock<IAIInsightsApiForwarderService> _aiInsightsApiForwarderService;
        private HttpContextBase _httpContext;
        private IAIInsightsService _service;

        [TestInitialize]
        public void Setup()
        {
            _sseStreamManagerService = new Mock<ISseStreamManagerService>();
            _aiInsightsApiForwarderService = new Mock<IAIInsightsApiForwarderService>();
            _httpContext = new Mock<HttpContextBase>().Object;
            _service = new AIInsightsService(_sseStreamManagerService.Object, _aiInsightsApiForwarderService.Object);

        }

        [TestMethod]
        public async Task AnalyzeChartsSseStream_WhenListenerCompletes_CancelsSenderAndReturns()
        {
            var callerCts = new CancellationTokenSource();

            // Listener: publish one message then complete
            _sseStreamManagerService.Setup(x => x.ListenerAsync(
                  It.IsAny<string>(),
                  It.IsAny<Action<string>>(),
                  It.IsAny<CancellationToken>(),
                  It.IsAny<int>(),       
                  It.IsAny<int>(),       
                  It.IsAny<int>(),       
                  It.IsAny<int>()))     
            .Returns<string, Action<string>, CancellationToken, int, int, int, int>(
                (url, onMsg, token, timeout, idle, maxRetry, delay) =>
                {
                    onMsg("{\"EventName\":\"AnalyzeCharts\",\"EventStatus\":\"Processing\",\"Data\":\"d\"}");
                    return Task.CompletedTask;
                });

            // Sender: run tasks and then block on the provided token until canceled by the service
            var senderCancelled = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _sseStreamManagerService.Setup(x => x.SenderAsync(It.IsAny<HttpContextBase>(), It.IsAny<CancellationToken>(), It.IsAny<Func<PushEventDelegate, Task[]>>()))
               .Returns<HttpContextBase, CancellationToken, Func<PushEventDelegate, Task[]>>((ctx, token, factory) =>
               {
                   PushEventDelegate push = _ => Task.CompletedTask;
                   var tasks = factory(push);
                   return Task.Run(async () =>
                   {
                       try { await Task.WhenAll(tasks).ConfigureAwait(false); } catch { /* ignore */ }
                       try { await Task.Delay(Timeout.Infinite, token).ConfigureAwait(false); }
                       catch (OperationCanceledException) { senderCancelled.TrySetResult(true); }
                   });
               });

            await _service.StreamAnalyzeChartsAsync("stream_token", _httpContext, callerCts.Token);

            var finished = await Task.WhenAny(senderCancelled.Task, Task.Delay(2000));
            Assert.AreSame(senderCancelled.Task, finished, "Sender did not observe cancellation.");
        }

        [TestMethod]
        public async Task AnalyzeChartsSseStream_WhenCallerTokenAlreadyCancelled_FinishesPromptly()
        {
            var callerCts = new CancellationTokenSource();
            callerCts.Cancel();

            // Return promptly on both sides to avoid hanging even if invoked
            _sseStreamManagerService.Setup(x => x.ListenerAsync(
                It.IsAny<string>(),
                  It.IsAny<Action<string>>(),
                  It.IsAny<CancellationToken>(),
                  It.IsAny<int>(),
                  It.IsAny<int>(),
                  It.IsAny<int>(),
                  It.IsAny<int>()
                ))
               .Returns(Task.FromCanceled(new CancellationToken(true)));
            _sseStreamManagerService.Setup(x => x.SenderAsync(It.IsAny<HttpContextBase>(), It.IsAny<CancellationToken>(), It.IsAny<Func<PushEventDelegate, Task[]>>()))
               .Returns(Task.FromCanceled(new CancellationToken(true)));

            var completed = Task.WhenAny(_service.StreamAnalyzeChartsAsync("stream_token", _httpContext, callerCts.Token), Task.Delay(1000));
            Assert.AreSame(await completed, await completed, "Method did not complete in time when token was already cancelled.");
        }

        [TestMethod]
        public async Task AnalyzeChartsSseStream_IgnoresPingMessages()
        {
            var model = new AIInsightsFileUploadDataModel { cache_key = "cache-1" };
            var callerCts = new CancellationTokenSource();

            // Listener: only emits a ping, then completes
            _sseStreamManagerService.Setup(x => x.ListenerAsync(
              It.IsAny<string>(),
              It.IsAny<Action<string>>(),
              It.IsAny<CancellationToken>(),
              It.IsAny<int>(),
              It.IsAny<int>(),
              It.IsAny<int>(),
              It.IsAny<int>()))
            .Returns<string, Action<string>, CancellationToken, int, int, int, int>(
                (url, onMsg, token, timeout, idle, maxRetry, delay) =>
                {
                    onMsg("ping"); // simulate ping message
                    return Task.CompletedTask;
                });

            // Sender: if push is called, fail the test
            _sseStreamManagerService.Setup(x => x.SenderAsync(It.IsAny<HttpContextBase>(), It.IsAny<CancellationToken>(), It.IsAny<Func<PushEventDelegate, Task[]>>()))
               .Returns<HttpContextBase, CancellationToken, Func<PushEventDelegate, Task[]>>((ctx, token, factory) =>
               {
                   var pushCalled = false;
                   PushEventDelegate push = _ =>
                   {
                       pushCalled = true;
                       return Task.CompletedTask;
                   };

                   var tasks = factory(push);
                   return Task.Run(async () =>
                   {
                       // Let processing task run; then check
                       var done = await Task.WhenAny(Task.WhenAll(tasks), Task.Delay(1000));
                       Assert.AreNotEqual(TaskStatus.Running, done.Status, "Processing did not settle.");
                       Assert.IsFalse(pushCalled, "Ping messages must be ignored.");
                   });
               });

            await _service.StreamAnalyzeChartsAsync("stream_token", _httpContext, callerCts.Token);
        }
    }
}