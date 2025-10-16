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
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
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
                var fileUploadResponse = GetUploadFileResponse(TabId);
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
                var fileResponse = GetUploadFileResponse(TabId);
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
                var fileResponse = GetUploadFileResponse(TabId);
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
                var cacheKey = GetUploadFileCacheKey(TabId);
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

        [HttpPost]
        public async Task<ActionResult> StartAnalyzeCharts()
        {
            object response = null;

            try
            {
                var fileResponse = GetUploadFileResponse(TabId);
                response = await _aiInsightsService.StartAnalyzeChartsAsync(fileResponse);
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - AI Insights StartAnalyzeCharts", ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AsyncTimeout(1800000)] // timeout 30 mintues

        public async Task StreamAnalyzeCharts(string streamToken, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(streamToken))
                {
                    throw new ArgumentException("StreamToken is required.", nameof(streamToken));
                }

                await _aiInsightsService.StreamAnalyzeChartsAsync(streamToken, HttpContext, cancellationToken);

            }
            catch (AggregateException agr)
            {
                LogExtension.LogError("CTS - AI Insights StreamAnalyzeCharts Inner Failed", agr);
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - AI Insights StreamAnalyzeCharts Failed", ex);
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

        private AIInsightsFileUploadDataModel GetUploadFileResponse(string tabId)
        {
            var cacheKey = GetUploadFileCacheKey(tabId);
            var fileResponse = _memoryCacheService.GetValue<AIInsightsFileUploadDataModel>(cacheKey);

            if (ConfigSettings.IsWriteLogInfoAIInsight)
            {
                LogExtension.LogInfo($"GetUploadFileResponse {cacheKey}: {JsonConvert.SerializeObject(fileResponse ?? new AIInsightsFileUploadDataModel())}");
            }

            return fileResponse;
        }

        private string GetUploadFileCacheKey(string tabId)
        {
            return string.Format(Consts.AIInsightsUploadFileKey, UserId, tabId);
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
