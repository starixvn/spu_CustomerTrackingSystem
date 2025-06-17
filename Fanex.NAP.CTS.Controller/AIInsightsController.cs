namespace Fanex.NAP.CTS.Controller
{
    using System;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Extensions;
    using Fanex.Logging;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model.AIInsights;
    using Fanex.NAP.CTS.Service;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;
    using Fanex.NAP.CTS.Model;
    using System.Collections.Generic;

    [NapSuperAuth]
    public class AIInsightsController : BaseController
    {       
        private readonly IAIInsightsService _aIInsightsService;
        private AIInsightsFileUploadModel _aiInsightsFileUploadModel;
        private readonly string UserID = "AIInsightsUserID";
        private const string ChatSessionKey = "ChatHistory";


        public AIInsightsController()
        {
            _aIInsightsService = new AIInsightsService();
            _aiInsightsFileUploadModel = new AIInsightsFileUploadModel();
            UserID = CurrentSession.UserId.ToString();
        }

        public ActionResult Index()
        {
            ViewBag.FileName = Session["UploadedFileName"] as string;

            return View();
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AIInsightsAnalyzeFile)]
        public ActionResult AnalyzeFile()
        {
            var data = new AIInsightsAnalyzeResponseModel();          

            try
            {
                var fileName = Session["UploadedFileName"] as string;               
                var fullPath = GetAnalysisFilePath(fileName);
                var fileBytes = GetUploadedFileBytes(fullPath);

                if (fileName == null || fileBytes == null)
                {
                    ViewBag.Message = "No file uploaded.";

                    return View();
                }             

                var fileContentBase64 = Convert.ToBase64String(fileBytes);

                var request = new AIInsightsAnalyzeRequestModel
                {
                    file_name = fileName,
                    file_content = fileContentBase64,
                    mode_type = 2,
                    changing_column_definition = "",
                    session_id = CurrentSession.SessionId,
                    language = "en"
                };

                data = _aIInsightsService.Analyze(request, ConfigSettings.AIInsightsAnalyzeUrl);                
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                                      .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                                      .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return Json(data.response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AIInsightsAskChatBot)]
        public ActionResult AskChatbot(AIInsightsChatbotRequestModel request)
        {
            var response = new AIInsightsChatbotResponseModel();

            try
            {
                request.session_id = CurrentSession.SessionId;
                
                response = _aIInsightsService.AskChatbot(request, ConfigSettings.AIInsightsChatBotUrl);

                SendMessage(request.question, response.response.answer);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return Json(response.response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LoadHistoryChatMessage()
        {
            var chatHistory = new List<ChatMessageModel>();

            try
            {
                chatHistory = Session[ChatSessionKey] as List<ChatMessageModel> ?? new List<ChatMessageModel>();
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
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
            var chatHistory = Session[ChatSessionKey] as List<ChatMessageModel> ?? new List<ChatMessageModel>();

            chatHistory.Add(new ChatMessageModel
            {
                Sender = sender,
                Message = message,
                Timestamp = DateTime.Now
            });

            Session[ChatSessionKey] = chatHistory;
        }        

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase uploadFile)
        {            
            if (uploadFile == null || uploadFile.ContentLength == 0)
            {              
                return RedirectToAction("Upload");
            }

            if (uploadFile.ContentLength > _aiInsightsFileUploadModel.MaxFileSizeBytes)
            {              
                return RedirectToAction("Upload");
            }

            var extension = Path.GetExtension(uploadFile.FileName).ToLower();

            if (!_aiInsightsFileUploadModel.AllowedExtensions.Contains(extension))
            {
                return RedirectToAction("Upload");
            }

            var fileName = Path.GetFileName(uploadFile.FileName);

            base.SaveUploadedFile(uploadFile);

            Session["UploadedFileName"] = fileName;

            ViewBag.FileName = fileName;

            return Content(fileName);          
        }
       
        private string GetAnalysisFilePath(string fileName)
        {
            var fullPath = Server.MapPath(Path.Combine(_aiInsightsFileUploadModel.TempFolder, fileName));

            return fullPath;
        }

        private byte[] GetUploadedFileBytes(string fullPath)
        {
            byte[] fileBytes;
            using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                fileBytes = new byte[fs.Length];
                fs.Read(fileBytes, 0, fileBytes.Length);
            }

            return fileBytes;
        }

    }
}
