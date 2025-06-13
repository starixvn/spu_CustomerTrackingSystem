using System.IO;
using System.Web;
using System.Web.Mvc;
using Fanex.NAP.Core.Web.Attribute;

namespace Fanex.NAP.CTS.Controller
{
    [NapSuperAuth]
    public class AIInsightsController : BaseController
    {
        public ActionResult Index()
        {
            var fileName = Session["UploadedFileName"] as string;
            var fileBytes = Session["UploadedFileBytes"] as byte[];

            if (fileName == null || fileBytes == null)
            {
                ViewBag.Message = "No file uploaded.";
                return View();
            }

            ViewBag.FileName = fileName;
            ViewBag.FileSize = fileBytes.Length;

            using (var ms = new MemoryStream(fileBytes))
            {
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
