using Fanex.NAP.CTS.Service.Implement.SseStream;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Fanex.NAP.CTS.Test.Services.SseStream
{
    [TestClass]
    public class SseSenderServiceTest
    {
        private SseSenderService _service;
        [TestInitialize]
        public void Setup()
        {
            _service = new SseSenderService();
        }

        [TestMethod]
        public async Task PushEventAsync_WhenTokenCancelled_ThrowsOperationCanceled()
        {
            var svc = _service.Start(FakeHttp());
            var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () =>
            {
                await svc.PushEventAsync(new { ok = 1 }, cts.Token);
            });
        }

        [TestMethod]
        public async Task PushEventAsync_WritesToOutput_WhenClientConnected()
        {
            var ms = new MemoryStream();
            var svc = _service.Start(FakeHttp(connected: true, output: ms));
            var ct = CancellationToken.None;

            await svc.PushEventAsync(new { hello = "world" }, ct);

            Assert.IsTrue(ms.Length > 0, "No bytes were written to the output stream.");
        }

        [TestMethod]
        public async Task Process_CancelsProcessing_WhenClientDisconnects()
        {
            var svc = _service.Start(FakeHttp(connected: false));
            var cts = new CancellationTokenSource();

            var cancelled = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            await svc.Process(push =>
            {
                return new[]
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            await Task.Delay(Timeout.Infinite, cts.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            cancelled.TrySetResult(true);
                        }
                    })
                };
            }, cts);

            await svc.Complete();

            var done = await Task.WhenAny(cancelled.Task, Task.Delay(1000));
            Assert.AreSame(cancelled.Task, done, "Processing task did not observe cancellation.");
        }

        [TestMethod]
        public async Task Process_ProcessingCompletesFirst_ThenCompleteReturnsCleanly()
        {
            var svc = _service.Start(FakeHttp(connected: true));
            var cts = new CancellationTokenSource();

            await svc.Process(push =>
            {
                // Processing completes immediately; keep-alive may still run until cancellation is requested internally
                return new[] { Task.CompletedTask };
            }, cts);

            // Ensure Complete does not throw even if cancellation happens during shutdown
            await svc.Complete();
        }

        private static HttpContextBase FakeHttp(bool connected = true, Stream output = null)
        {
            var response = new Mock<HttpResponseBase>();
            response.SetupGet(r => r.IsClientConnected).Returns(connected);
            response.SetupGet(r => r.OutputStream).Returns(output ?? new MemoryStream());

            var server = new Mock<HttpServerUtilityBase>();

            var ctx = new Mock<HttpContextBase>();
            ctx.SetupGet(c => c.Response).Returns(response.Object);
            ctx.SetupGet(c => c.Server).Returns(server.Object);
            return ctx.Object;
        }
    }
}