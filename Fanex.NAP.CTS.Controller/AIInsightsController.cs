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
    using static Fanex.NAP.CTS.Resource.Consts.Consts;
    using Fanex.NAP.CTS.Model;

    [NapSuperAuth]
    public class AIInsightsController : BaseController
    {
        private readonly IAIInsightsService _aIInsightsService;

        public AIInsightsController()
        {
            _aIInsightsService = new AIInsightsService();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AIInsightsAnalyzeFile)]
        public ActionResult AnalyzeFile()
        {           
            try
            {
                //var fileName = Session["UploadedFileName"] as string;
                //var fileBytes = Session["UploadedFileBytes"] as byte[];

                //if (fileName == null || fileBytes == null)
                //{
                //    ViewBag.Message = "No file uploaded.";

                //    return View();
                //}

                //var fileContentBase64 = Convert.ToBase64String(fileBytes);

                //var request = new AIInsightsAnalyzeRequestModel
                //{
                //    data_path = fileName,
                //    file_content = fileContentBase64,
                //    mode_type = 2,
                //    changing_column_definition = ""
                //};
          
                //var response = _aIInsightsService.Analyze(request, ConfigSettings.AIInsightsAnalyzeUrl);

            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                                  .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                                  .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return View();
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

    }
}
