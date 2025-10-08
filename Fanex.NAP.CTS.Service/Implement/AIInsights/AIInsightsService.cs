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
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    public class AIInsightsService : IAIInsightsService
    {
        #region Fields
        private readonly IAIInsightsApiForwarderService _apiForwarderService;
        private readonly ISseStreamManagerService _sseStreamManagerService;
        private AIInsightsFileUploadModel _aiInsightsFileUploadModel;

        private const int CHART_PROCESSING_DELAY_MS = 500; // Delay between processing each chart in milliseconds
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

            // Initialize SSE stream
            _sseStreamManagerService.Start(httpContext);

            // Process charts analysis
            var analyzeChartsTask = ProcessAnalyzeChartsAsync(model, linkedCts.Token);
            await _sseStreamManagerService.Process(new[] { analyzeChartsTask }, linkedCts.Token);

            // Complete SSE stream
            await _sseStreamManagerService.Complete(linkedCts);
        }

        private async Task ProcessAnalyzeChartsAsync(AIInsightsFileUploadDataModel model, CancellationToken cancellationToken)
        {
            try
            {
                await SendAnalysisEventAsync(AIInsightEventName.AnalyzeCharts, AIInsightEventStatus.Started, cancellationToken);

                var analysisResult = await AnalyzeChartsAsync(model, cancellationToken);

                if (IsAnalysisSuccessful(analysisResult))
                {
                    await ProcessChartsSequentially(analysisResult.response.visualization_response.chart_infos, cancellationToken);

                    await SendAnalysisEventAsync(AIInsightEventName.AnalyzeCharts, AIInsightEventStatus.Completed, cancellationToken);
                }
                else
                {
                    await SendAnalysisEventAsync(AIInsightEventName.AnalyzeCharts, AIInsightEventStatus.Failed, cancellationToken);
                    throw new InvalidOperationException($"Chart analysis failed: {analysisResult?.error}");
                }
            }
            catch (OperationCanceledException)
            {
                // Normal cancellation - no logging needed
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - AI Insights AnalyzeChartsAsync", ex);
                await SendErrorEventAsync(cancellationToken);
            }
        }

        private bool IsAnalysisSuccessful(AIInsightsAnalyzeChartsResponseModel result)
        {
            return result?.status_code != -1
                && result?.response?.visualization_response?.chart_infos != null;
        }

        private async Task ProcessChartsSequentially(IEnumerable<dynamic> chartInfos, CancellationToken cancellationToken)
        {
            foreach (var chart in chartInfos)
            {
                await Task.Delay(CHART_PROCESSING_DELAY_MS, cancellationToken);

                await SendChartProcessingEventAsync(chart, cancellationToken);
            }
        }

        private async Task<AIInsightsAnalyzeChartsResponseModel> AnalyzeChartsAsync(AIInsightsFileUploadDataModel model, CancellationToken cancellationToken)
        {
            var request = new AIInsightsAnalyzeRequestModel
            {
                cache_key = model.cache_key
            };

            WriteLog($"Request: {nameof(AnalyzeChartsSseStream)}", request);

            var result = await _apiForwarderService.AnalyzeCharts(request, ConfigSettings.AIInsightsAnalyzeChartsUrl, cancellationToken);

            WriteLog($"Response: {nameof(AnalyzeChartsSseStream)}", result);

            ValidateResponse(result);

            return result;
        }

        private async Task SendAnalysisEventAsync(AIInsightEventName eventName, AIInsightEventStatus status, CancellationToken cancellationToken)
        {
            var eventData = new AIInsightEventSourceViewModel(eventName, status);
            await _sseStreamManagerService.PushEventAsync(eventData, cancellationToken);
        }

        private async Task SendChartProcessingEventAsync(dynamic chart, CancellationToken cancellationToken)
        {
            var eventData = new AIInsightEventSourceViewModel(AIInsightEventName.AnalyzeCharts, AIInsightEventStatus.Processing, chart);
            await _sseStreamManagerService.PushEventAsync(eventData, cancellationToken);
        }

        private async Task SendErrorEventAsync(CancellationToken cancellationToken)
        {
            var eventData = new AIInsightEventSourceViewModel(AIInsightEventName.Error, AIInsightEventStatus.Failed);
            await _sseStreamManagerService.PushEventAsync(eventData, cancellationToken);
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
