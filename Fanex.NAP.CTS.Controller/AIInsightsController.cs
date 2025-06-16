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

    [NapSuperAuth]
    public class AIInsightsController : BaseController
    {
        private readonly string tempFolder = "~/TempFiles";
        private const int maxFileSizeBytes = 5 * 1024 * 1024; // 5 MB
        private readonly string[] allowedExtensions = { ".xls" };

        private readonly IAIInsightsService _aIInsightsService;

        public AIInsightsController()
        {
            _aIInsightsService = new AIInsightsService();
        }

        public ActionResult Index()
        {
            ViewBag.FileName = Session["UploadedFileName"] as string;

            return View();
        }

        [HttpPost]
        //[TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AIInsightsAnalyzeFile)]
        public ActionResult AnalyzeFile()
        {
            var data = new AIInsightsAnalyzeResponseModel();          

            try
            {
                var fileName = Session["UploadedFileName"] as string;
                var fileBytes = Session["UploadedFileBytes"] as byte[];

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
                    session_id = "1",
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
        public ActionResult AskChatbot(AIInsightsChatbotRequestModel request)
        {
            var response = new AIInsightsChatbotResponseModel();

            try
            {
                response = _aIInsightsService.AskChatbot(request, ConfigSettings.AIInsightsChatBotUrl);
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
        public ActionResult UploadFile(HttpPostedFileBase uploadFile)
        {
            if (uploadFile != null && uploadFile.ContentLength > 0 && uploadFile.FileName.EndsWith(".xls"))
            {
                string fileName = Path.GetFileName(uploadFile.FileName);

                Session["UploadedFileName"] = fileName;
                ViewBag.FileName = fileName;

                return Content(fileName);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase uploadFile)
        {           
            CleanupOldTempFiles();

            if (uploadFile == null || uploadFile.ContentLength == 0)
            {              
                return RedirectToAction("Upload");
            }

            if (uploadFile.ContentLength > maxFileSizeBytes)
            {              
                return RedirectToAction("Upload");
            }

            var extension = Path.GetExtension(uploadFile.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                return RedirectToAction("Upload");
            }

            
            var fileName = Guid.NewGuid() + extension;
            var fullPath = Server.MapPath(Path.Combine(tempFolder, fileName));
          
            Directory.CreateDirectory(Server.MapPath(tempFolder));
           
            uploadFile.SaveAs(fullPath);

         
            Session["AnalysisFilePath"] = fullPath;

            Session["UploadTime"] = DateTime.Now;

            TempData["Message"] = "Upload Successful.";

            return RedirectToAction("Analyze");
        }

        public ActionResult Analyze()
        {
            var filePath = Session["AnalysisFilePath"] as string;
            var uploadTime = Session["UploadTime"] as DateTime?;

            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                return Content("Does not exit a file.");

            if (uploadTime == null || DateTime.Now - uploadTime > TimeSpan.FromHours(1))
            {
               
                System.IO.File.Delete(filePath);
                Session.Remove("AnalysisFilePath");

                return Content("a file expired or deleted.");
            }
          
            var lines = System.IO.File.ReadAllLines(filePath);

            return Content("Successful: " + lines.Length + " rows.");
        }

        private void CleanupOldTempFiles()
        {
            string path = Server.MapPath(tempFolder);
            if (!Directory.Exists(path)) return;

            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                try
                {
                    var creationTime = System.IO.File.GetCreationTime(file);
                    if (DateTime.Now - creationTime > TimeSpan.FromHours(1))
                    {
                        System.IO.File.Delete(file);
                    }
                }
                catch 
                { 
                
                }
            }
        }

    }
}
