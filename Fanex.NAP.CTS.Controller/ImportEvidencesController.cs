namespace Fanex.NAP.CTS.Controller
{   
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using System.Linq;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NAP.CTS.ViewModel.ImportEvidences;
    using System.Collections.Generic;
    using Fanex.Logging;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.CTS.Model.ImportEvidences;

    [NapAuth(FunctionKey = FSFunctionKeys.ImportEvidences, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class ImportEvidencesController : BaseController
    {
        private readonly IImportEvidencesService _importEvidencesService;

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public ImportEvidencesController() 
            => _importEvidencesService = new ImportEvidencesService();

        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.ImportEvidences,
                            NAP.Core.Business.Constant.Policy.Host);

            return View(isHostPermission);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.ImportEvidences, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [HttpPost]
        public ActionResult ImportFromExcel()
        {
            var file = Request.Files[0] as HttpPostedFileBase;
            var fileName = file.FileName;
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (fileExtension == ".xls")
                _importEvidencesService.ParseFileXls(file.InputStream);
            else
                _importEvidencesService.ParseFileXlsx(file.InputStream);

            var resultCheckAccountEvidenceList = _importEvidencesService.CheckRuleAddAccountEvidence();

            var accountEvidenceNotExistList = resultCheckAccountEvidenceList
                .Where(x => x.RowStatus != string.Empty)
                .Select(x => x);

            return accountEvidenceNotExistList.Any()
                ? ReturnResultCheckAccountEvidence(fileName, accountEvidenceNotExistList)
                : ReturnResultCheckAccountEvidence(fileName, accountEvidenceNotExistList);
                //: InsertAccountEvidence(fileName, resultCheckAccountEvidenceList);
        }

        private ActionResult ReturnResultCheckAccountEvidence(string fileName, IEnumerable<ResultCheckAccountEvidenceModel> accountEvidenceNotExistList)
        {
            var resultCheckAccountEvidenceList = accountEvidenceNotExistList.Select(x => new ResultCheckAccountEvidenceViewModel
            {
                RowId = x.RowId,
                RowStatus = x.RowStatus,
                CTSCustID = x.CTSCustID,
                EvidenceID = x.EvidenceID
            });

            var resultImport = new ResultImportViewModel
            {
                FileName = fileName,
                ResultCheckAccountEvidenceList = resultCheckAccountEvidenceList
            };

            return Json(resultImport, JsonRequestBehavior.AllowGet);
        }

        private ActionResult InsertAccountEvidence(string fileName, IEnumerable<ResultCheckAccountEvidenceModel> resultCheckAccountEvidenceList)
        {
            var resultImport = new ResultImportViewModel
            {
                FileName = fileName
            };

            var resultInsertDB = _importEvidencesService.InsertAccountEvidence(resultCheckAccountEvidenceList, CurrentSession.UserId, true);

            if (resultInsertDB.ErrorCode != 0)
            {
                LogService.Error(string.Format("File name: {0} import with content {1} - error: {2}", fileName, "", resultInsertDB.ErrorMsg));
            }

            return Json(resultImport, JsonRequestBehavior.AllowGet);
        }
    }
}