namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.AIInsights;
    using Fanex.NAP.CTS.Model.AIInsights.Api;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Implement.AIInsights;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Service.Interface.AIInsights;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [NapSuperAuth]
    public class AIInsightsController : BaseController
    {
        private readonly IAIInsightsService _aIInsightsService;
        private readonly IMemoryCacheService _memoryCacheService;
        private string UserId => CurrentSession?.UserId.ToString();
        private string TabId => Request?.Headers["X-Tab-Id"]?.ToString();


        public AIInsightsController()
        {
            _aIInsightsService = new AIInsightsService();
            _memoryCacheService = new MemoryCacheService();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetFileName()
        {
            var fileUploadResponse = GetUploadFileResponse();
            var fileName = fileUploadResponse?.file_name ?? string.Empty;

            return Json(fileName, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AIInsightsAnalyzeFile)]
        public async Task<ActionResult> AnalyzeSummary()
        {
            var data = new AIInsightsAnalyzeSummaryDataModel();

            try
            {
                var fileResponse = GetUploadFileResponse();
                data = await _aIInsightsService.AnalyzeSummary(fileResponse);
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - AI Insights AnalyzeSummary", ex);
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> AnalyzeCharts()
        {
            var data = new AIInsightsAnalyzeChartsDataModel();

            try
            {
                var fileResponse = GetUploadFileResponse();
                data = await _aIInsightsService.AnalyzeCharts(fileResponse);
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - AI Insights AnalyzeCharts", ex);
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AIInsightsAskChatBot)]
        public async Task<ActionResult> AskChatbot(string question)
        {
            var response = new AIInsightsChatbotDataModel();

            try
            {
                var fileResponse = GetUploadFileResponse();
                response = await _aIInsightsService.AskChatbot(fileResponse, question);

                SendMessage(question, response.answer);
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - AI Insights AskChatbot", ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LoadHistoryChatMessage()
        {
            var chatHistory = new List<ChatMessageModel>();

            try
            {
                chatHistory = _memoryCacheService.GetValue<List<ChatMessageModel>>(string.Format(Consts.AIInsightsChatHistoryKey, UserId, TabId)) ?? new List<ChatMessageModel>();
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - AI Insights Load History Chat Message", ex);
            }

            return Json(chatHistory, JsonRequestBehavior.AllowGet);
        }

        private void SendMessage(string userMessage, string botMessage)
        {
            SaveMessage("u", userMessage);

            SaveMessage("b", botMessage);
        }

        private void SaveMessage(string sender, string message)
        {
            var chatHistory = _memoryCacheService.GetValue<List<ChatMessageModel>>(string.Format(Consts.AIInsightsChatHistoryKey, UserId, TabId));

            chatHistory.Add(new ChatMessageModel
            {
                Sender = sender,
                Message = message,
                Timestamp = DateTime.Now
            });

            _memoryCacheService.PushValueBySlidingTime(string.Format(Consts.AIInsightsChatHistoryKey, UserId, TabId), chatHistory, 1);
        }

        [HttpPost]
        public async Task<ActionResult> UploadFile(HttpPostedFileBase uploadFile)
        {
            try
            {
                var result = await _aIInsightsService.UploadFile(uploadFile, UserId);
                _memoryCacheService.PushValueBySlidingTime(string.Format(Consts.AIInsightsUploadFileKey, UserId, TabId), result?.response, 1);

                return Json(new
                {
                    success = true,
                    message = result
                }, JsonRequestBehavior.AllowGet);
            }
            catch (InvalidDataException invalidDataException)
            {
                return Json(new
                {
                    success = false,
                    message = invalidDataException.Message
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - AI Insights Upload File", ex);

                return Json(new
                {
                    success = false,
                    message = "CTS - AI Insights Upload File"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private AIInsightsFileUploadDataModel GetUploadFileResponse()
        {
            var fileResponse = _memoryCacheService.GetValue<AIInsightsFileUploadDataModel>(string.Format(Consts.AIInsightsUploadFileKey, UserId, TabId));

            return fileResponse;
        }
    }
}
