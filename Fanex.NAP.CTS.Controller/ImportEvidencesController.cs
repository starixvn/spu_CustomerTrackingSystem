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
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using System.Threading.Tasks;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Criteria.Account;
    using Fanex.NAP.CTS.Core.Helpers.Xml;

    [NapAuth(FunctionKey = FSFunctionKeys.ImportEvidences, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class ImportEvidencesController : BaseController
    {
        private const string ImportTemplate = "~/App_Data/Excel Template/ImportEvidences/template.xlsx";

        private readonly IImportEvidencesService _importEvidencesService;

        private readonly ICustomerEvidenceService _customerEvidenceService;

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public ImportEvidencesController()
        {
            _importEvidencesService = new ImportEvidencesService();
            _customerEvidenceService = new CustomerEvidenceService();
        }

        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.ImportEvidences,
                            NAP.Core.Business.Constant.Policy.Host);

            var isOverwritePermission = userService.IsHavingPermission(CurrentSession.UserId,
                          NapSettings.Settings.SiteId,
                          FSFunctionKeys.OverwriteEvidences,
                          NAP.Core.Business.Constant.Policy.Read);

            return View(new EvidencePermissionViewModel
            {
                IsHostPermission = isHostPermission,
                IsOverwritePermission = isOverwritePermission
            });
        }

        public ActionResult DownloadTemplateExcel()
        {
            var template = Server.MapPath(ImportTemplate);

            var file = _importEvidencesService.DownloadTemplateExcel(template);

            return ExcelFile(file, "Template");
        }

        [NapAuth(FunctionKey = FSFunctionKeys.ImportEvidences, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [HttpPost]
        public ActionResult ImportFromExcel(bool isOverwrite)
        {
            IUserService userService = new UserService();
            var isOverwritePermission = userService.IsHavingPermission(CurrentSession.UserId,
                    NapSettings.Settings.SiteId,
                    FSFunctionKeys.OverwriteEvidences,
                    NAP.Core.Business.Constant.Policy.Read);

            //case: being updated overwrite per while using import evidence feature
            isOverwrite = isOverwritePermission ? isOverwrite : false;   

            var file = Request.Files[0] as HttpPostedFileBase;
            var fileName = file.FileName;
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            var errorParseFile = fileExtension == ".xls"
                ? _importEvidencesService.ParseFileXls(file.InputStream, fileName)
                : _importEvidencesService.ParseFileXlsx(file.InputStream, fileName);

            if (errorParseFile == string.Empty)
            {
                var resultCheckAccountEvidenceList = _importEvidencesService.CheckRuleAddAccountEvidence(isOverwrite);

                var accountEvidenceErrorList = resultCheckAccountEvidenceList
                    .Where(x => x.RowStatus != string.Empty)
                    .Select(x => x);

                if (!accountEvidenceErrorList.Any())
                { 
                    InsertAccountEvidence(fileName, CurrentSession.UserId, resultCheckAccountEvidenceList, isOverwrite);

                    var associatedAccountList = _customerEvidenceService.GetAssociatedAccounts(
                        new GetAssociatedAccountByCTSCustIdsCriteria
                        {
                            CTSCustIdList = string.Join(",", resultCheckAccountEvidenceList.Select(c => c.CTSCustID).ToList())
                        });

                    if (associatedAccountList.Any())
                    {
                        _customerEvidenceService.UpdateBetLimitOfFlaggedCustomer(
                            new UpdateBetLimitByEvidenceAddingCriteria
                            {
                                CustIDList = string.Join(",", associatedAccountList)
                            });
                    }
                }

                return ReturnResultCheckAccountEvidence(fileName, accountEvidenceErrorList);
            }

            return Json(new ResultImportViewModel {
                FileName = fileName,
                ErrorCode = 2,
                ErrorMessage = errorParseFile
            }, JsonRequestBehavior.AllowGet);
        }

        private ActionResult ReturnResultCheckAccountEvidence(string fileName, IEnumerable<ResultCheckAccountEvidenceModel> accountEvidenceErrorList)
        {
            var resultCheckAccountEvidenceList = new List<ResultCheckAccountEvidenceViewModel>();
            var validateException = accountEvidenceErrorList.FirstOrDefault(x => x.RowId.Equals(-1));

            if (validateException == null)
            {
                var rawAccountEvidenceList = _importEvidencesService.GetRawAccountEvidences();

                resultCheckAccountEvidenceList = accountEvidenceErrorList
                        .Join(rawAccountEvidenceList, x => x.RowId, y => y.RowId,
                        (x, y) => new ResultCheckAccountEvidenceViewModel
                        {
                            RowId = x.RowId,
                            RowStatus = x.RowStatus,
                            Username = y.Username,
                            EvidenceCode = y.EvidenceCode
                        }).ToList();
            }
            else
            {
                resultCheckAccountEvidenceList.Add(new ResultCheckAccountEvidenceViewModel
                {
                    RowId = 0,
                    RowStatus = validateException.RowStatus,
                    Username = string.Empty,
                    EvidenceCode = string.Empty
                });
            }

            return Json(new ResultImportViewModel {
                FileName = fileName,
                ResultCheckAccountEvidenceList = resultCheckAccountEvidenceList,
                ErrorCode = accountEvidenceErrorList.Any() ? 1 : -1
            }, JsonRequestBehavior.AllowGet);
        }

        private void InsertAccountEvidence(string fileName, int userId, IEnumerable<ResultCheckAccountEvidenceModel> resultCheckAccountEvidenceList, bool isOverride)
        {
            Task.Run(() =>
            {
                var template = Server.MapPath(ImportTemplate);
                _importEvidencesService.InsertAccountEvidence(resultCheckAccountEvidenceList, fileName, userId, template, isOverride);
            });
        }
    }
}
