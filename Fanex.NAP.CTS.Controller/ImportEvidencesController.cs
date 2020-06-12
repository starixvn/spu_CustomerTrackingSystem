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
    using System;
    using System.Threading.Tasks;

    [NapAuth(FunctionKey = FSFunctionKeys.ImportEvidences, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class ImportEvidencesController : BaseController
    {
        private const string ImportTemplate = "~/App_Data/Excel Template/ImportEvidences/template.xlsx";
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

        public ActionResult DownloadTemplateExcel()
        {
            var template = Server.MapPath(ImportTemplate);

            var file = _importEvidencesService.DownloadTemplateExcel(template);

            return ExcelFile(file, "Template");
        }

        [NapAuth(FunctionKey = FSFunctionKeys.ImportEvidences, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [HttpPost]
        public ActionResult ImportFromExcel()
        {
            var file = Request.Files[0] as HttpPostedFileBase;
            var fileName = file.FileName;
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            var errorParseFile = fileExtension == ".xls"
                ? _importEvidencesService.ParseFileXls(file.InputStream, fileName)
                : _importEvidencesService.ParseFileXlsx(file.InputStream, fileName);

            if (errorParseFile == string.Empty)
            {
                var resultCheckAccountEvidenceList = _importEvidencesService.CheckRuleAddAccountEvidence();

                var accountEvidenceErrorList = resultCheckAccountEvidenceList
                    .Where(x => x.RowStatus != string.Empty)
                    .Select(x => x);

                if (!accountEvidenceErrorList.Any())
                    InsertAccountEvidence(fileName, CurrentSession.UserId, resultCheckAccountEvidenceList);
                
                return ReturnResultCheckAccountEvidence(fileName, accountEvidenceErrorList);
            }

            return Json(new ResultImportViewModel {
                FileName = fileName,
                ErrorCode = 1,
                ErrorMessage = errorParseFile
            }, JsonRequestBehavior.AllowGet);
        }

        private ActionResult ReturnResultCheckAccountEvidence(string fileName, IEnumerable<ResultCheckAccountEvidenceModel> accountEvidenceErrorList)
        {
            var rawAccountEvidenceList = _importEvidencesService.GetRawAccountEvidences();

            var resultCheckAccountEvidenceList = accountEvidenceErrorList
                    .Join(rawAccountEvidenceList, x => x.RowId, y => y.RowId,
                    (x, y) => new ResultCheckAccountEvidenceViewModel
                    {
                        RowId = x.RowId,
                        RowStatus = x.RowStatus,
                        Username = y.Username,
                        EvidenceCode = y.EvidenceCode
                    });

            return Json(new ResultImportViewModel {
                FileName = fileName,
                ResultCheckAccountEvidenceList = resultCheckAccountEvidenceList,
                ErrorCode = accountEvidenceErrorList.Any() ? 1 : -1
            }, JsonRequestBehavior.AllowGet);
        }

        private void InsertAccountEvidence(string fileName, int userId, IEnumerable<ResultCheckAccountEvidenceModel> resultCheckAccountEvidenceList)
        {
            Task.Run(() =>
            {
                var template = Server.MapPath(ImportTemplate);
                _importEvidencesService.InsertAccountEvidence(resultCheckAccountEvidenceList, fileName, userId, template);
            });
        }
    }
}
