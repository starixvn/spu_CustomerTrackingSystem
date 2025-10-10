namespace Fanex.NAP.CTS.Service.Implement.AIInsights
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model.AIInsights;
    using Fanex.NAP.CTS.Model.AIInsights.Api;
    using Fanex.NAP.CTS.Service.AIInsights;
    using Fanex.NAP.CTS.Service.Implement.SseStream;
    using Fanex.NAP.CTS.Service.Interface.AIInsights;
    using Fanex.NAP.CTS.Service.Interface.SseStream;
    using Fanex.NAP.CTS.ViewModel.AIInsights;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using static Fanex.NAP.CTS.Service.Delegate.SseDelegate;

    public class AIInsightsService : IAIInsightsService
    {
        #region Fields
        private readonly IAIInsightsApiForwarderService _apiForwarderService;
        private readonly ISseStreamManagerService _sseStreamManagerService;
        private AIInsightsFileUploadModel _aiInsightsFileUploadModel;
        #endregion

        #region constructor
        public AIInsightsService()
        {
            _sseStreamManagerService = new SseStreamManagerService();
            _apiForwarderService = new AIInsightsApiForwarderService();
            _aiInsightsFileUploadModel = new AIInsightsFileUploadModel();
        }
        #endregion

        #region analyze charts with sse stream

        public async Task AnalyzeChartsSseStream(AIInsightsFileUploadDataModel model, HttpContextBase httpContext, CancellationToken cancellationToken)
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var messageQueue = new BlockingCollection<string>();

            var listenerTask = Task.Run(
                async () => await _sseStreamManagerService.ListenerAsync(
                    ConfigSettings.AIInsightsAnalyzeChartsUrl,
                    onMessage: msg => { messageQueue.TryAdd(msg, 100, linkedCts.Token); },
                    timeOutbySeconds: ConfigSettings.AIAnalyzeRequestTimeout,
                    linkedCts.Token)
                .ConfigureAwait(false),
                linkedCts.Token);

            var senderTask = Task.Run(
                async () => await _sseStreamManagerService.SenderAsync(
                    httpContext,
                    linkedCts.Token,
                    (pushMsgDelegate) =>
                        new Task[] { ProcessAnalyzeChartsAsync(messageQueue, pushMsgDelegate, linkedCts.Token) })
                .ConfigureAwait(false),
                linkedCts.Token);

            await Task.WhenAny(listenerTask, senderTask).ConfigureAwait(false);
            linkedCts.Cancel();
        }

        private async Task ProcessAnalyzeChartsAsync(BlockingCollection<string> msgQueue, PushEventDelegate pushMsgDelegate, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var msg in msgQueue.GetConsumingEnumerable(cancellationToken))
                {
                    await SendChartProcessingEventAsync(msg, pushMsgDelegate).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // Normal cancellation - no logging needed
            }
            catch (ArgumentException arg)
            {
                LogExtension.LogError("CTS - AI Insights AnalyzeChartsAsync", arg);
                await pushMsgDelegate(new AIInsightEventSourceViewModel(AIInsightEventName.Error, AIInsightEventStatus.Failed));
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - AI Insights AnalyzeChartsAsync", ex);
                await pushMsgDelegate(new AIInsightEventSourceViewModel(AIInsightEventName.Error, AIInsightEventStatus.Failed));
            }
        }

        private async Task SendChartProcessingEventAsync(string chart, PushEventDelegate pushMsgDelegate)
        {
            if (chart.Contains("ping")) return;

            var model = JsonConvert.DeserializeObject<AIInsightEventSourceViewModel>(chart);

            if (model.EventName == AIInsightEventName.AnalyzeCharts.ToString())
            {
                switch (model.EventStatus)
                {
                    case nameof(AIInsightEventStatus.Processing):
                        {
                            await pushMsgDelegate(
                                new AIInsightEventSourceViewModel(
                                    AIInsightEventName.AnalyzeCharts,
                                    AIInsightEventStatus.Processing,
                                    model.Data));
                            break;
                        }
                    default:
                        {
                            if (Enum.TryParse(model.EventStatus, out AIInsightEventStatus eventStatus))
                            {
                                await pushMsgDelegate(
                                    new AIInsightEventSourceViewModel(
                                        AIInsightEventName.AnalyzeCharts,
                                        eventStatus));
                            }
                            break;
                        }
                }
            }
        }
        #endregion

        public async Task<AIInsightsAnalyzeSummaryResponseModel> AnalyzeSummary(AIInsightsFileUploadDataModel model)
        {
            var request = new AIInsightsAnalyzeRequestModel
            {
                cache_key = model?.cache_key,
            };

            WriteLog($"Request: {nameof(AnalyzeSummary)}", request);

            var result = await _apiForwarderService.AnalyzeSummary(request, ConfigSettings.AIInsightsAnalyzeSummaryUrl);

            WriteLog($"Response: {nameof(AnalyzeSummary)}", result);

            ValidateResponse(result);
            return result;
        }

        public async Task<AIInsightsChatbotDataModel> AskChatbot(AIInsightsFileUploadDataModel model, string question)
        {
            var request = new AIInsightsChatbotRequestModel
            {
                cache_key = model?.cache_key,
                question = question
            };

            WriteLog($"Request: {nameof(AskChatbot)}", request);

            var result = await _apiForwarderService.AskChatbot(request, ConfigSettings.AIInsightsChatBotUrl);

            WriteLog($"Response: {nameof(AskChatbot)}", result);

            ValidateResponse(result);
            return result.response;
        }

        public async Task<AIInsightsFileUploadResponseModel> UploadFile(HttpPostedFileBase uploadFile, string session_id)
        {
            if (uploadFile == null || uploadFile.ContentLength == 0)
            {
                throw new InvalidDataException($"Content cannot be empty.");
            }

            if (uploadFile.ContentLength > _aiInsightsFileUploadModel.MaxFileSizeBytes)
            {
                throw new InvalidDataException($"Exceeds the allowed limit of {_aiInsightsFileUploadModel.MaxFileSizeBytes / (1024 * 1024)} MB.");
            }

            var request = new AIInsightsFileUploadRequestModel
            {
                file_base64image = uploadFile.FileBaseToBase64StringChunked(),
                file_name = Path.GetFileName(uploadFile.FileName),
                session_id = session_id
            };

            WriteLog($"Request: {nameof(UploadFile)}", $"{request.session_id}_{request.file_name}");

            var result = await _apiForwarderService.UploadFile(request, ConfigSettings.AIInsightsUploadUrl);

            WriteLog($"Response: {nameof(UploadFile)}", result);

            ValidateResponse(result);
            return result;
        }

        private void ValidateResponse(AIInsightResponseModel response)
        {
            if (response != null && response.status_code > 299)
            {
                throw new HttpListenerException(response.status_code, response.error);
            }
        }

        private void WriteLog(string functionName, object data)
        {
            if (ConfigSettings.IsWriteLogInfoAIInsight)
            {
                var jsonData = data != null ? JsonConvert.SerializeObject(data) : "null";
                LogExtension.LogInfo($"CTS - AI Insights {functionName}: {jsonData}");
            }
        }
    }
}
