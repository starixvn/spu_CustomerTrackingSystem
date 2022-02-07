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
    using Fanex.NAP.CTS.ViewModel.Common;
    using Newtonsoft.Json;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Service.Interface;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;
    using Fanex.NAP.CTS.Model;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.EvidenceManagement, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class EvidenceManagementController : BaseController
    {
        private const string ImportTemplate = "~/App_Data/Excel Template/ImportEvidences/template.xlsx";

        private readonly IImportEvidencesService _importEvidencesService;

        private readonly ICommonService _commonService;

        private readonly ICustomerEvidenceService _customerEvidenceService;

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public EvidenceManagementController()
        {
            _importEvidencesService = new ImportEvidencesService();
            _customerEvidenceService = new CustomerEvidenceService();
            _commonService = new CommonService();
        }

        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.EvidenceManagement,
                            NAP.Core.Business.Constant.Policy.Host);

            var isOverwritePermission = userService.IsHavingPermission(CurrentSession.UserId,
                          NapSettings.Settings.SiteId,
                          FSFunctionKeys.OverwriteEvidences,
                          NAP.Core.Business.Constant.Policy.Read);

            if(!isHostPermission)
            {
                return Redirect("Errors/403");
            }


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

        [NapAuth(FunctionKey = FSFunctionKeys.EvidenceManagement, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingUserAccessFilterAttribute(systemId = SystemId.CTSWeb, functionId = MonitorCtsFunctionId.EvidenceManagementImport)]
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
                    InsertCustomerEvidence(fileName, CurrentSession.UserId, resultCheckAccountEvidenceList, isOverwrite);                   
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

        private void InsertCustomerEvidence(string fileName, int userId, IEnumerable<ResultCheckAccountEvidenceModel> resultCheckAccountEvidenceList, bool isOverride)
        {
            Task.Run(() =>
            {
                var template = Server.MapPath(ImportTemplate);
                _importEvidencesService.InsertCustEvidenceFromFile(resultCheckAccountEvidenceList, fileName, userId, template, isOverride);
            });
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.EvidenceManagement, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult ValidateAccountsForRemoveEvidences(string userNames)
        {
            var isException = false;
            var viewModel = new ValidatedAccountsViewModel { IsException = false };

            if (!string.IsNullOrEmpty(userNames))
            {
                var userNameList = userNames.Split(',');
                var userNameToUpperList = userNameList.Select(x => x.ToUpper());

                var duplicateToUpperList = userNameToUpperList.GroupBy(x => x)
                       .Where(g => g.Count() > 1)
                       .Select(x => x.Last());

                var noDuplicateToUpperList = userNameToUpperList.Distinct();
                var noDuplicateToUpperUserName = string.Join(",", noDuplicateToUpperList);

                var validatedList = _commonService.ValidateUsernames(noDuplicateToUpperUserName, out isException);
                var validatedToUpperList = validatedList.Select(x => new ValidateUserNameModel
                {
                    CTSCustID = x.CTSCustID,
                    CustID = x.CustID,
                    UserName = x.UserName.ToUpper()
                });

                viewModel = GenerateValidatedViewModel(userNames, validatedToUpperList, isException);

                viewModel.UserNameDuplicatedList = userNameList.Where(x=> duplicateToUpperList.Contains(x.ToUpper()));
            }

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        private ValidatedAccountsViewModel GenerateValidatedViewModel(string userNames, IEnumerable<ValidateUserNameModel> validatedToUpperList, bool isException)
        {
            var userNameList = userNames.Split(',');
            var userNameToUpperList = userNameList.Select(x => x.ToUpper());
            var invalidList = Enumerable.Empty<string>();

            if (!isException)
            {
                var userNameToUpperValidatedList = validatedToUpperList.Select(x => x.UserName.ToUpper());

                invalidList = userNameList.Where(x => !userNameToUpperValidatedList.Contains(x.ToUpper()));
            }
            else
                invalidList = userNameList;

            return new ValidatedAccountsViewModel
            {
                IsException = isException,
                UserNameInvalidList = invalidList
            };
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.EvidenceManagement, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingUserAccessFilterAttribute(systemId = SystemId.CTSWeb, functionId = MonitorCtsFunctionId.EvidenceManagementRemove)]
        public ActionResult InsertAccountsForRemoveEvidences(string userNames)
        {
            var insertResult = new InsertAccountsForRemoveEvidenceViewModel { ErrorCode = 0 };

            var isException = false;

            if (!string.IsNullOrEmpty(userNames))
            {
                var userNameList = userNames.Split(',');

                var duplicateList = userNameList.GroupBy(x => x)
                       .Where(g => g.Count() > 1)
                       .Select(x => x.Last());

                var newUserNames = string.Join(",", userNameList.Distinct());

                var validatedList = _commonService.ValidateUsernames(newUserNames, out isException);

                insertResult.ValidatedResult = GenerateValidatedViewModel(userNames, validatedList, isException);
                insertResult.ValidatedResult.UserNameDuplicatedList = duplicateList;
                var isAllowInsert = !insertResult.ValidatedResult.UserNameInvalidList.Any() && !insertResult.ValidatedResult.UserNameDuplicatedList.Any();

                if (isAllowInsert)
                {
                    var custEvidenceList = validatedList.Select(x => new InsertAccountEvidenceQueueModel
                    {
                        CTSCustID = x.CTSCustID,
                        CustID = x.CustID,
                        Remark = string.Empty
                    });

                    var custEvidencesJson = JsonConvert.SerializeObject(custEvidenceList);

                    var dbResult = _importEvidencesService.InsertAccountEvidencesQueue(custEvidencesJson, CurrentSession.UserId);

                    insertResult.ErrorCode = dbResult.ErrorCode;
                    insertResult.ErrorMessage = dbResult.ErrorMsg;
                }
            }            

            return Json(insertResult, JsonRequestBehavior.AllowGet);
        }

    }
}
