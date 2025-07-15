namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.AIInsights;
    using Fanex.NAP.CTS.Service;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
    public class AIInsightsController : BaseController
    {       
        private readonly IAIInsightsService _aIInsightsService;
        private AIInsightsFileUploadModel _aiInsightsFileUploadModel;
        private const string ChatSessionKey = "ChatHistory";


        public AIInsightsController()
        {
            _aIInsightsService = new AIInsightsService();
            _aiInsightsFileUploadModel = new AIInsightsFileUploadModel();
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
                var fileContentBase64 = ConfigSettings.AIInsightsStoreFile
                        ? GetFileContent(fileName)
                        : Session["UploadedFileToBase64String"] as string;

                if (fileName == null || string.IsNullOrEmpty(fileContentBase64))
                {
                    ViewBag.Message = "No file uploaded.";

                    return View();
                }

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
                data.response = new AIInsightsAnalyzeDataModel
                {
                    ErrorCode = -1,
                    ErrorMessage = ex.Message
                };

                LogExtension.LogError("CTS - AI Insights Analyze File", ex);
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
                request.file_name = Session["UploadedFileName"] as string;
                request.file_content = ConfigSettings.AIInsightsStoreFile
                        ? GetFileContent(request.file_name)
                        : Session["UploadedFileToBase64String"] as string;

                response = _aIInsightsService.AskChatbot(request, ConfigSettings.AIInsightsChatBotUrl);

                SendMessage(request.question, response.response.answer);
            }
            catch (Exception ex)
            {                
                LogExtension.LogError("CTS - AI Insights AskChatbot", ex);         
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
                return Json(new
                {
                    success = false,
                    message = $"Content cannot be empty."
                }, JsonRequestBehavior.AllowGet);
            }

            if (uploadFile.ContentLength > _aiInsightsFileUploadModel.MaxFileSizeBytes)
            {
                return Json(new
                {
                    success = false,
                    message = $"Exceeds the allowed limit of {_aiInsightsFileUploadModel.MaxFileSizeBytes} MB."
                }, JsonRequestBehavior.AllowGet);               
            }

            if (ConfigSettings.AIInsightsStoreFile)
            {
                base.SaveUploadedFile(uploadFile);
            }
            else
            {
                using (var ms = new MemoryStream())
                {
                    uploadFile.InputStream.CopyTo(ms);                   
                    Session["UploadedFileToBase64String"] = Convert.ToBase64String(ms.ToArray());
                }
            }

            var fileName = Path.GetFileName(uploadFile.FileName);

            Session["UploadedFileName"] = fileName;

            ViewBag.FileName = fileName;

            return Json(new
            {
                success = true,
                message = fileName
            }, JsonRequestBehavior.AllowGet);
        }

        private string GetFileContent(string fileName)
        {           
            var fullPath = GetAnalysisFilePath(fileName);
            var fileBytes = GetUploadedFileBytes(fullPath);

            return Convert.ToBase64String(fileBytes);
        }

        private string GetAnalysisFilePath(string fileName)
        {
            var userSubFolder = Path.Combine(_aiInsightsFileUploadModel.TempFolder, CurrentSession.UserId.ToString());

            var fullPath = Server.MapPath(Path.Combine(userSubFolder, fileName));

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
