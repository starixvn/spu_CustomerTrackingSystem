using Fanex.NAP.CTS.Service.Implement.SseStream;
using Fanex.NAP.CTS.Service.Interface.SseStream;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using static Fanex.NAP.CTS.Service.Delegate.SseDelegate;

namespace Fanex.NAP.CTS.Test.Services.SseStream
{
    [TestClass]
    public class SseStreamManagerServiceTest
    {
        private Mock<ISseListenerService> _listener;
        private Mock<ISseSenderService> _sender;
        private SseStreamManagerService _manager;
        private HttpContextBase _httpContext;

        [TestInitialize]
        public void Setup()
        {
            _listener = new Mock<ISseListenerService>(MockBehavior.Strict);
            _sender = new Mock<ISseSenderService>(MockBehavior.Strict);
            _manager = new SseStreamManagerService(_listener.Object, _sender.Object);
            _httpContext = new Mock<HttpContextBase>().Object;
        }

        [TestMethod]
        public async Task ListenerAsync_ForwardsArgs_AndAwaits()
        {
            var url = "http://example/sse";
            Action<string> onMsg = _ => { };
            var timeout = 5;
            var ct = new CancellationTokenSource().Token;

            _listener.Setup(x => x.ListenerAsync(url, onMsg, ct, timeout, 3, 2000))
                     .Returns<string, Action<string>, CancellationToken, int, int, int>(
                        (u, m, t, to, max, delay) => Task.CompletedTask);

            await _manager.ListenerAsync(url, onMsg, ct, timeout,
                             maxRetryCount: 3,
                             retryDelayMs: 2000);

            _listener.VerifyAll();
        }

        [TestMethod]
        public async Task SenderAsync_Starts_Processes_Completes_WithSameToken()
        {
            var callerCts = new CancellationTokenSource();
            var capturedToken = default(CancellationToken);
            Func<PushEventDelegate, Task[]> capturedFactory = null;

            _sender.Setup(s => s.Start(_httpContext))
                   .Returns(_sender.Object);

            _sender.Setup(s => s.Process(It.IsAny<Func<PushEventDelegate, Task[]>>(), It.IsAny<CancellationTokenSource>()))
                   .Callback<Func<PushEventDelegate, Task[]>, CancellationTokenSource>((f, lcts) =>
                   {
                       capturedFactory = f;
                       capturedToken = lcts.Token;
                   })
                   .Returns(Task.FromResult(_sender.Object));

            _sender.Setup(s => s.Complete())
                   .Returns(Task.FromResult(_sender.Object));

            await _manager.SenderAsync(_httpContext, callerCts.Token, push => Array.Empty<Task>());

            Assert.IsNotNull(capturedFactory, "Factory was not forwarded.");
            Assert.IsTrue(capturedToken.CanBeCanceled, "Linked token was not created.");
            _sender.VerifyAll();
        }

        [TestMethod]
        public async Task SenderAsync_CancellationFromCaller_CancelsSenderProcessing()
        {
            var callerCts = new CancellationTokenSource();
            var processingCancelled = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            _sender.Setup(s => s.Start(_httpContext)).Returns(_sender.Object);

            _sender.Setup(s => s.Process(It.IsAny<Func<PushEventDelegate, Task[]>>(), It.IsAny<CancellationTokenSource>()))
                   .Returns<Func<PushEventDelegate, Task[]>, CancellationTokenSource>(async (factory, lcts) =>
                   {
                       // Simulate a task that observes cancellation
                       var t = Task.Run(async () =>
                       {
                           try
                           {
                               await Task.Delay(Timeout.Infinite, lcts.Token);
                           }
                           catch (OperationCanceledException)
                           {
                               processingCancelled.TrySetResult(true);
                           }
                       }, lcts.Token);

                       await Task.Delay(50);
                       callerCts.Cancel(); // cancel caller; manager creates a linked CTS
                       return _sender.Object;
                   });

            _sender.Setup(s => s.Complete()).Returns(Task.FromResult(_sender.Object));

            await _manager.SenderAsync(_httpContext, callerCts.Token, push => Array.Empty<Task>());

            var done = await Task.WhenAny(processingCancelled.Task, Task.Delay(1000));
            Assert.AreSame(processingCancelled.Task, done, "Processing did not observe cancellation.");
        }
    }
}