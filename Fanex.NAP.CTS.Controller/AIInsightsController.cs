namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.AIInsights;
    using Fanex.NAP.CTS.Model.AIInsights.Api;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Implement.AIInsights;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Service.Interface.AIInsights;
    using Fanex.NAP.CTS.ViewModel.AIInsights;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [NapSuperAuth]
    public class AIInsightsController : BaseController
    {
        #region Constants and Configuration

        private const int CACHE_EXPIRATION_HOURS = 3;
        private const int KEEP_ALIVE_INTERVAL_SECONDS = 5;
        private const int CHART_PROCESSING_DELAY_MS = 500;
        private const int ANALYSIS_COMPLETION_DELAY_MS = 1000;
        private const string CONTENT_TYPE_EVENT_STREAM = "text/event-stream";
        private const string CACHE_CONTROL_NO_CACHE = "no-cache";
        private const string KEEP_ALIVE_MESSAGE = "event: keepalive\ndata: ping\n\n";

        #endregion

        #region Fields and Properties

        private readonly IAIInsightsService _aiInsightsService;
        private readonly IMemoryCacheService _memoryCacheService;

        private string UserId => CurrentSession?.UserId.ToString();
        private string TabId => Request?.Headers["X-Tab-Id"]?.ToString();

        #endregion

        #region Constructor

        public AIInsightsController()
        {
            _aiInsightsService = new AIInsightsService();
            _memoryCacheService = new RedisCacheService();
        }

        #endregion

        #region Public Actions

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetFileName()
        {
            try
            {
                var fileUploadResponse = GetUploadFileResponse();
                var fileName = fileUploadResponse?.file_name ?? string.Empty;

                return Json(fileName, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - AI Insights GetFileName", ex);
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AIInsightsAnalyzeFile)]
        public async Task<ActionResult> AnalyzeSummary()
        {
            var data = new AIInsightsAnalyzeSummaryResponseModel();

            try
            {
                var fileResponse = GetUploadFileResponse();
                data = await _aiInsightsService.AnalyzeSummary(fileResponse);
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - AI Insights AnalyzeSummary", ex);
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AIInsightsAskChatBot)]
        public async Task<ActionResult> AskChatbot(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                return Json(new AIInsightsChatbotDataModel { answer = "Please provide a valid question." }, JsonRequestBehavior.AllowGet);
            }

            var response = new AIInsightsChatbotDataModel();

            try
            {
                var fileResponse = GetUploadFileResponse();
                response = await _aiInsightsService.AskChatbot(fileResponse, question);

                await SaveChatMessageAsync(question, response.answer);
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - AI Insights AskChatbot", ex);
                response.answer = "Sorry, I encountered an error while processing your question.";
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LoadHistoryChatMessage()
        {
            try
            {
                var chatHistory = GetChatHistory();
                return Json(chatHistory, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - AI Insights Load History Chat Message", ex);
                return Json(new List<ChatMessageModel>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadFile(HttpPostedFileBase uploadFile)
        {
            if (uploadFile == null || uploadFile.ContentLength == 0)
            {
                return CreateErrorResponse("No file provided or file is empty.");
            }

            try
            {
                var result = await _aiInsightsService.UploadFile(uploadFile, UserId);
                var cacheKey = GetUploadFileCacheKey();
                await _memoryCacheService.PushValueAsync(cacheKey, result?.response, CACHE_EXPIRATION_HOURS);

                return CreateSuccessResponse(result);
            }
            catch (InvalidDataException ex)
            {
                return CreateErrorResponse(ex.Message);
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - AI Insights Upload File", ex);
                return CreateErrorResponse("An error occurred while uploading the file.");
            }
        }

        [HttpGet]
        public async Task StreamAnalyzeCharts(string tabId, CancellationToken cancellationToken)
        {
            try
            {
                Request?.Headers.Add("X-Tab-Id", tabId);
                ConfigureServerSentEvents();
                var fileResponse = GetUploadFileResponse();

                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    var maintainTask = MaintainConnectionAsync(cts);
                    var analysisTask = AnalysisChartsAsync(fileResponse, cts.Token);

                    await Task.WhenAny(analysisTask, maintainTask);
                }
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - AI Insights StreamAnalyzeCharts", ex);
            }
        }
        #endregion

        #region Private Methods

        #region Chat Management

        private async Task SaveChatMessageAsync(string userMessage, string botMessage)
        {
            await SaveMessage("u", userMessage);
            await SaveMessage("b", botMessage);
        }

        private async Task SaveMessage(string sender, string message)
        {
            var chatHistory = GetChatHistory();
            chatHistory.Add(new ChatMessageModel
            {
                Sender = sender,
                Message = message,
                Timestamp = DateTime.Now
            });

            var cacheKey = GetChatHistoryCacheKey();
            await _memoryCacheService.PushValueAsync(cacheKey, chatHistory, CACHE_EXPIRATION_HOURS);
        }

        private List<ChatMessageModel> GetChatHistory()
        {
            var cacheKey = GetChatHistoryCacheKey();
            return _memoryCacheService.GetValue<List<ChatMessageModel>>(cacheKey) ?? new List<ChatMessageModel>();
        }

        private string GetChatHistoryCacheKey()
        {
            return string.Format(Consts.AIInsightsChatHistoryKey, UserId, TabId);
        }

        #endregion

        #region File Management

        private AIInsightsFileUploadDataModel GetUploadFileResponse()
        {
            var cacheKey = GetUploadFileCacheKey();
            var fileResponse = _memoryCacheService.GetValue<AIInsightsFileUploadDataModel>(cacheKey);

            if (ConfigSettings.IsWriteLogInfoAIInsight)
            {
                LogExtension.LogInfo($"GetUploadFileResponse {cacheKey}: {JsonConvert.SerializeObject(fileResponse ?? new AIInsightsFileUploadDataModel())}");
            }

            return fileResponse;
        }

        private string GetUploadFileCacheKey()
        {
            return string.Format(Consts.AIInsightsUploadFileKey, UserId, TabId);
        }

        #endregion

        #region Server-Sent Events

        private void ConfigureServerSentEvents()
        {
            var response = HttpContext.Response;

            response.ContentType = CONTENT_TYPE_EVENT_STREAM;
            response.CacheControl = CACHE_CONTROL_NO_CACHE;
            response.BufferOutput = false;
        }

        private async Task MaintainConnectionAsync(CancellationTokenSource cts)
        {
            while (HttpContext.Response.IsClientConnected && !cts.IsCancellationRequested)
            {
                try
                {
                    await SendKeepAliveAsync();
                    await Task.Delay(TimeSpan.FromSeconds(KEEP_ALIVE_INTERVAL_SECONDS));
                }
                catch (TaskCanceledException)
                {
                    // Task was cancelled - exit the loop
                    break;
                }
            }

            if (!HttpContext.Response.IsClientConnected)
            {
                // SSE connection closed by client.
                cts.Cancel();
            }
        }

        private async Task SendKeepAliveAsync()
        {
            await PushEventAsync(KEEP_ALIVE_MESSAGE);
        }

        private async Task PushEventAsync<T>(T data, CancellationToken token = default)
        {
            if (token != null)
            {
                token.ThrowIfCancellationRequested();
            }

            if ((HttpContext.Response == null || !HttpContext.Response.IsClientConnected))
                return;

            try
            {
                string json = JsonConvert.SerializeObject(data);
                string message = $"data: {json}\n\n";

                byte[] buffer = Encoding.UTF8.GetBytes(message);
                await HttpContext.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                await HttpContext.Response.OutputStream.FlushAsync();
            }
            catch (HttpException ex) when (ex.ErrorCode == -2147023419 || ex.Message.Contains("The remote host closed the connection"))
            {
                // Client disconnected - this is normal behavior, don't log as error
                // Error code -2147023419 (0x800704CD) indicates client disconnected
            }
            catch (ObjectDisposedException)
            {
                // Response object was disposed (client disconnected) - normal behavior
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("disconnected") || ex.Message.Contains("closed"))
            {
                // Client connection was closed - normal behavior
            }
            catch (Exception ex)
            {
                // Only log actual errors, not normal disconnections
                LogExtension.LogError($"CTS - AI Insights PushEvent for tabId: {TabId}", ex);
            }
        }

        #endregion

        #region Analysis Processing

        private async Task AnalysisChartsAsync(AIInsightsFileUploadDataModel fileResponse, CancellationToken cancellationToken)
        {
            try
            {
                await ProcessAnalyzeChartsAsync(fileResponse, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // cancellation requested - normal behavior
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - AI Insights ProcessAnalysis", ex);
                await PushEventAsync(new AIInsightEventSourceViewModel(AIInsightEventName.Error, AIInsightEventStatus.Failed));
            }
        }

        private async Task ProcessAnalyzeChartsAsync(AIInsightsFileUploadDataModel fileResponse, CancellationToken cancellationToken)
        {
            await PushEventAsync(new AIInsightEventSourceViewModel(AIInsightEventName.AnalyzeCharts, AIInsightEventStatus.Started), cancellationToken);

            var dataCharts = await AnalyzeChartsAsync(fileResponse, cancellationToken);

            if (dataCharts?.status_code != -1 && dataCharts?.response?.visualization_response?.chart_infos != null)
            {
                await ProcessChartsSequentially(dataCharts.response.visualization_response.chart_infos, cancellationToken);
                await PushEventAsync(new AIInsightEventSourceViewModel(AIInsightEventName.AnalyzeCharts, AIInsightEventStatus.Completed), cancellationToken);
            }
            else
            {
                await PushEventAsync(new AIInsightEventSourceViewModel(AIInsightEventName.AnalyzeCharts, AIInsightEventStatus.Failed), cancellationToken);
                throw new InvalidOperationException("Analyze Charts failed");
            }
        }

        private async Task ProcessChartsSequentially(IEnumerable<dynamic> chartInfos, CancellationToken cancellationToken)
        {
            foreach (var chart in chartInfos)
            {
                await Task.Delay(CHART_PROCESSING_DELAY_MS);
                await PushEventAsync(new AIInsightEventSourceViewModel(AIInsightEventName.AnalyzeCharts, AIInsightEventStatus.Processing, chart), cancellationToken);
            }
        }

        private async Task<AIInsightsAnalyzeSummaryResponseModel> AnalyzeSummaryAsync(AIInsightsFileUploadDataModel fileResponse)
        {
            try
            {
                return await _aiInsightsService.AnalyzeSummary(fileResponse);
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - AI Insights AnalyzeSummary", ex);
                return new AIInsightsAnalyzeSummaryResponseModel { status_code = -1 };
            }
        }

        private async Task<AIInsightsAnalyzeChartsResponseModel> AnalyzeChartsAsync(AIInsightsFileUploadDataModel fileResponse, CancellationToken cancellationToken)
        {
            try
            {
                return await _aiInsightsService.AnalyzeCharts(fileResponse, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                //  indicates cancellation by calller
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - AI Insights AnalyzeCharts", ex);
            }

            return new AIInsightsAnalyzeChartsResponseModel { status_code = -1 };
        }

        #endregion

        #region Response Helpers

        private ActionResult CreateSuccessResponse(object data)
        {
            return Json(new
            {
                success = true,
                message = data
            }, JsonRequestBehavior.AllowGet);
        }

        private ActionResult CreateErrorResponse(string message)
        {
            return Json(new
            {
                success = false,
                message = message
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion
    }
}
