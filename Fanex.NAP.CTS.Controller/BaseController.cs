namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Context;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Context;
    using Fanex.NAP.Core.Web.Session;
    using Fanex.NAP.CTS.Core.ExportExcel;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.AIInsights;
    using Fanex.NAP.CTS.Resource.Consts;
    using NPOI.SS.UserModel;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;

    public class BaseController : Controller
    {
        
        public BaseController()
        {
            CurrentContext = new CurrentContext();
            UserService = new UserService();

            if (NapSettings.Settings != null)
            {
                CurrentSession = new CurrentSession();
                ViewBag.Session = CurrentSession;
            }           
        }

        public virtual string BaseControllerName { get => Consts.BaseController; }

        public int SiteId { get; } = NapSettings.Settings == null ? 0 : NapSettings.Settings.SiteId;

        public bool IsSiteLogin { get; } = (NapSettings.Settings != null) && NapSettings.Settings.EnableDirectLogin;

        public string Environment { get; } = NapSettings.Settings == null ? string.Empty : NapSettings.Settings.Environment;

        public ICurrentContext CurrentContext
        {
            get;
            set;
        }

        public ICurrentSession CurrentSession
        {
            get;
            set;
        }

        public IUserService UserService { get; }

        /// <summary>
        /// Return the HTML string of a View.
        /// </summary>
        /// <param name="context">Controller context.</param>
        /// <param name="viewPath">View path.</param>
        /// <param name="viewModel">View model.</param>
        /// <param name="partial">View partial.</param>
        /// <returns>HTML string.</returns>
        public string RenderViewToString(
            ControllerContext context,
            string viewPath,
            object viewModel = null,
            bool partial = false)
        {
            ViewEngineResult viewEngineResult = null;
            viewEngineResult = partial ? ViewEngines.Engines.FindPartialView(context, viewPath) : ViewEngines.Engines.FindView(context, viewPath, null);

            if (viewEngineResult == null)
            {
                throw new FileNotFoundException("View cannot be found.");
            }

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = viewModel;

            var htmlStringFromView = string.Empty;
            using (var stringWriter = new StringWriter())
            {
                var viewContext = new ViewContext(
                    context,
                    view,
                    context.Controller.ViewData,
                    context.Controller.TempData,
                    stringWriter);
                view.Render(viewContext, stringWriter);
                htmlStringFromView = stringWriter.ToString();
            }

            return htmlStringFromView;
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            var logMessage = new LogMessage(filterContext.Exception.Message)
                .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(filterContext.Exception))
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(filterContext.Exception)).ToString();

            LogService.Error(logMessage);

            base.OnException(filterContext);
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = int.MaxValue
            };
        }

        protected ActionResult ExcelFile(byte[] contents, string fileName, string extension = "xlsx")
        {
            return File(contents, "application/vnd.ms-excel", fileName + "." + extension);
        }

        protected new ActionResult Json(object data)
        {
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        protected ActionResult JData<T>(IEnumerable<T> data)
        {
            return Json(new { data });
        }

        protected static byte[] RenderExcelFile(string template, Action<IWorkbook> render)
        {
            var wb = ExcelUtilities.GetWorkbook(template);

            render(wb);

            return wb.ToBytes();
        }

        protected static byte[] RenderExcelFile<TLegend, TData, TSummary>(string template, FSExcelConfig<TLegend, TData, TSummary> config, TLegend legend, IEnumerable<TData> data, TSummary summary)
        {
            return ExcelUtilities.GetWorkbook(template)
                .RenderSheet(config, legend, data, summary)
                .ToBytes();
        }

        protected void SaveUploadedFile(HttpPostedFileBase uploadFile)
        {
            var aiInsightsFileUploadModel = new AIInsightsFileUploadModel();

            CleanupOldTempFiles(aiInsightsFileUploadModel, TimeSpan.FromHours(1));

            var fileName = Path.GetFileName(uploadFile.FileName);

            SaveUploadedFile(uploadFile, fileName, aiInsightsFileUploadModel);

            Session["UploadedFileName"] = fileName;

            ViewBag.FileName = fileName;
        }

        protected void SaveUploadedFile(HttpPostedFileBase uploadFile, string fileName, AIInsightsFileUploadModel aiInsightsFileUploadModel)
        {
            var userID = CurrentSession.UserId.ToString();

            var userSubFolder = Path.Combine(aiInsightsFileUploadModel.TempFolder, userID);

            var serverUserFolderPath = Server.MapPath(userSubFolder);

            Directory.CreateDirectory(serverUserFolderPath);
           
            var fullPath = Path.Combine(serverUserFolderPath, fileName);          

            uploadFile.SaveAs(fullPath);

            Session["UploadTime"] = DateTime.Now;

            Session["UploadedFileName"] = fileName;
        }

        protected void CleanupOldTempFiles(AIInsightsFileUploadModel aiInsightsFileUploadModel, TimeSpan maxAge)
        {
            var userID = CurrentSession.UserId.ToString();

            var userSubFolder = Path.Combine(aiInsightsFileUploadModel.TempFolder, userID);

            var folderPath = Server.MapPath(userSubFolder);           

            if (!Directory.Exists(folderPath)) 
                return;

            var files = Directory.GetFiles(folderPath);

            foreach (var file in files)
            {
                try
                {
                    var creationTime = System.IO.File.GetCreationTime(file);

                    if (DateTime.Now - creationTime > maxAge)
                    {
                        System.IO.File.Delete(file);
                    }
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                                     .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                                     .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error(logMessage);
                }
            }
        }

    }
}