namespace Fanex.NAP.CTS.Controller
{   
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using System;
    using System.Linq;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NAP.CTS.ViewModel.ImportEvidences;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Logging;

    [NapAuth(FunctionKey = FSFunctionKeys.ImportEvidences, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class ImportEvidencesController : BaseController
    {
        private readonly IImportEvidencesService _importEvidencesService;

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public ImportEvidencesController() 
            => _importEvidencesService = new ImportEvidencesService();

        public ActionResult Index()
        {
            return View();
        }

        [NapAuth(FunctionKey = FSFunctionKeys.ImportEvidences, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [HttpPost]
        public ActionResult ImportFromExcel()
        {
            var file = Request.Files[0] as HttpPostedFileBase;
            var fileName = file.FileName;
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            var importFileResult = new ImportFileResultViewModel
            {
                FileName = fileName,
                CheckRuleResultList = null
            };

            var evidenceAddList = fileExtension == ".xls"
                ? _importEvidencesService.ParseFileXls(file.InputStream)
                : _importEvidencesService.ParseFileXlsx(file.InputStream);

            var checkRuleList = _importEvidencesService.CheckRule(evidenceAddList);

            if (checkRuleList.Any())
            {
                importFileResult.CheckRuleResultList = checkRuleList;

                return Json(importFileResult, JsonRequestBehavior.AllowGet);
            }

            var insertDbResult = _importEvidencesService.AddEvidenceList(evidenceAddList);

            if (insertDbResult.ErrorCode != 0)
            {
                LogService.Error(string.Format("File name: {0} import with content {1} - error: {2}", file.FileName, "", insertDbResult.ErrorMsg));
            }

            return Json(importFileResult, JsonRequestBehavior.AllowGet);
        }      
    }
}