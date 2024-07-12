namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.CustomerClassification.ProblemAccount;
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using Fanex.NAP.CTS.ViewModel.Common;
    using Fanex.NAP.CTS.ViewModel.ProblemAccountManagement;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.ProblemAccountManagement, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class ProblemAccountManagementController : BaseController
    {
        private const string ProblemAccountTemplate = "~/App_Data/Excel Template/ProblemAccountManagement/ExportTemplate.xlsx";
        private readonly IProblemAccountManagementService _pamService;
        private readonly IProblemAccountClassificationService _paClassificationService;
        private readonly IRescanCustomerClassificationService _rescanClassificationService;
        private readonly ICustomerClassificationService _customerClassificationService;
        private readonly ICommonService _commonService;
        public override string BaseControllerName { get => Consts.MenuItemController; }

        public ProblemAccountManagementController()
        {
            _pamService = new ProblemAccountManagementService();
            _paClassificationService = new ProblemAccountClassificationService();
            _rescanClassificationService = new RescanCustomerClassificationService();
            _customerClassificationService = new CustomerClassificationService();
            _commonService = new CommonService();
        }

        public ActionResult Index()
        {
            var result = new ProblemAccountViewModel();

            var sites = _commonService.GetSites(false);

            if (sites != null && sites.Any())
            {
                result.SiteGroupList = sites.GroupBy(g => g.SubscriberGroupName)
                             .Select(x => new { SubscriberGroupName = x.Key, SiteList = x.ToList() })
                             .Select(x => new SiteGroupViewModel
                             {
                                 SubscriberGroupName = x.SubscriberGroupName,
                                 SiteList = x.SiteList.Select(y => new SiteViewModel
                                 {
                                     SiteID = y.SiteID,
                                     SiteName = y.SiteName,
                                     IsSelected = true
                                 })
                             }).ToList();
            }

            var categories = _pamService.GetAllCategory();

            if (categories != null && categories.Any())
            {
                result.CategoryList = categories
                       .Where(x => x.CategoryGroup == Consts.ProblemAccountManagement_CategoryPA)
                       .GroupBy(g => new { g.CategoryGroup })
                       .Select(x => new { Group = x.Key, CategoryList = x.ToList() })
                       .Select(x => new CategoryGroupViewModel
                       {
                           CategoryGroupName = x.Group.CategoryGroup,
                           CategoryList = x.CategoryList.Select(y => new CategoryViewModel
                           {
                               CategoryIDs = y.CategoryIDs,
                               CategoryName = y.CategoryName
                           }).OrderBy(y => y.CategoryName)
                       }).ToList();
            }

            return View(result);
        }

        [HttpPost]
        public ActionResult Index(string usernames)
        {
            ViewBag.UserNames = usernames;
            return Index();
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.ProblemAccountManagementRpt)]
        public ActionResult GetProblemAccount(ProblemAccountFilterViewModel filter)
        {
            var problemAccounts = GetProblemAccounts(filter);

            return Json(new
            {
                data = problemAccounts
            }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<GetProblemAccountViewModel> GetProblemAccounts(ProblemAccountFilterViewModel filter)
        {
            var problemAccounts = Enumerable.Empty<GetProblemAccountViewModel>();

            if (filter.ToDate != DateTime.MinValue)
            {
                filter.ToDate = new DateTime(filter.ToDate.Year, filter.ToDate.Month, filter.ToDate.Day, 23, 59, 59);
            }

            var paModel = _pamService.GetProblemAccounts(filter);

            if (paModel != null && paModel.Any())
            {
                problemAccounts = paModel.OrderByDescending(x => x.CreatedDate).Select(x => new GetProblemAccountViewModel(x));                
            }

            return problemAccounts;
        }

        [HttpGet]
        public ActionResult GetProblemAccountAllCategories()
        {
            var data = _pamService.GetProblemAccountCategories(true).OrderBy(x => x.Priorty);              

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ValidateProblemAccountInput(ValidateProblemAccountInputViewModel model)
        {
            var result = new ResultValidatedViewModel();

            var validatedModels = _pamService.ValidateProblemAccounts(model);

            if (validatedModels != null && validatedModels.Any())
            {
                result = GetResultValidated(validatedModels);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.ProblemAccountManagementMarkNew)]
        public ActionResult SaveMarkProblemAccount(ValidateProblemAccountInputViewModel model)
        {
            var result = new ResultSaveProblemAccountViewModel();
            var dbResult = new DBResult();

            var validatedModels = _pamService.ValidateProblemAccounts(model);

            if (validatedModels != null && validatedModels.Any())
            {
                var resultValidate = GetResultValidated(validatedModels);

                if (!resultValidate.ProblemAccountInvalidList.Any())
                {
                    if (model.IsRobot)
                    {
                        dbResult = SaveMarkRobotAccount(model, validatedModels);
                    }
                    else
                    {
                        dbResult = SaveMarkPA(model, validatedModels);                        
                    }

                    result.ErrorCode = dbResult.ErrorCode;
                    result.ErrorMessage = dbResult.ErrorMsg;
                    result.ResultValidated = resultValidate;
                }
            }           

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private DBResult SaveMarkPA(ValidateProblemAccountInputViewModel model, IEnumerable<ProblemAccountValidatedModel> validatedModels)
        {
            var pas = validatedModels.Select(x => new InsertProblemAccountModel
            {
                CustID = x.CustID,
                CTSCustID = x.CTSCustID,
                RoleID = x.RoleID,
                SubscriberID = x.SubscriberID,
                IsLicensee = x.IsLicensee,
                CategoryID = model.CategoryID,
                Remark = model.Comment,
                IsMarkedDirectly = true,
                IsFromCTS = true,
                CreatedBy = CurrentSession.UserId
            });

            return _paClassificationService.InsertProblemAccountPackage(pas, CCInputFlow.InsertPACategory, "ProblemAccountManagement", FromFlow.CTSWEB, true);
        }

        private DBResult SaveMarkRobotAccount(ValidateProblemAccountInputViewModel model, IEnumerable<ProblemAccountValidatedModel> validatedModels)
        {
            var robotAccounts = validatedModels.Select(x => new InsertRobotModel {
                CustID = x.CustID,
                CategoryID = model.CategoryID,
                Remark = model.Comment,
                IsMarkedDirectly = true,
                IsFromCTS = true,
                CreatedBy = CurrentSession.UserId                
            });

            var custInfos = _paClassificationService.InsertRobotAccountPackge(robotAccounts, false);

            if (custInfos.Any())
            {
                var custIds = custInfos.Select(x => x.CustID).Distinct();

                _customerClassificationService.PushDataToExternalPackge(custIds, new PushDataToExternalModel
                {
                    ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,                                   
                    IsUpdateDangerToMainDB = true,
                    IsSyncAFC = false
                }, FromFlow.CTSWEB);

                return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
            }
            else
            {
                return new DBResult { ErrorCode = -1, ErrorMsg = "Mark PA Robot failed" };
            }                           
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.ProblemAccountManagementMarkNew)]
        public ActionResult SaveUnmarkProblemAccount(ValidateProblemAccountInputViewModel model)
        {
            var result = new ResultSaveProblemAccountViewModel();

            var validatedModels = _pamService.ValidateProblemAccounts(model);

            if (validatedModels != null && validatedModels.Any())
            {
                var resultValidate = GetResultValidated(validatedModels);

                if (!resultValidate.ProblemAccountInvalidList.Any())
                {
                    var resultUnmark = _paClassificationService.UnmarkProblemAccount(validatedModels.Select(x => new UnmarkProblemAccountModel
                    {
                        CustID = x.CustID,
                        CTSCustID = x.CTSCustID,
                        RoleID = x.RoleID,
                        SubscriberID = x.SubscriberID,
                        CreatedBy = CurrentSession.UserId,
                        Remark = model.Comment,
                        IsExceptDirectDownline = model.IsUnmarkDownlines,
                        IsMarkedDirectly = true
                    }), false);

                    if (resultUnmark.CustInfos != null && resultUnmark.CustInfos.Any())
                    {
                        _rescanClassificationService.RescanCustomerClassification(new RescanCustClassificationModel
                        {
                            CustIds = resultUnmark.CustInfos.Select(x => x.CustID),
                            FunctionId = FunctionId.UnmarkCustomerCategory,
                            IsUpdateDangerToMainDB = true
                        });
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.ProblemAccountManagementMarkNew)]
        public ActionResult ExportToExcel(ProblemAccountFilterViewModel filter)
        {
            var templatePath = Server.MapPath(ProblemAccountTemplate);            
            filter.DateTime = DateTime.UtcNow.AddHours(-4);

            var fileName = string.Format("PAM_{0}_{1}.xlsx"
                , filter.DateTime.ToString(FormatDateTimeToString_YYYYmmdd)
                , filter.DateTime.ToString(Consts.FormatTimeToString_hhmmss));
          
            var problemAccounts = GetProblemAccounts(filter);
            
            var excelFile = _pamService.ExportProblemAccount(problemAccounts, fileName, templatePath, filter);

            return File(excelFile, "application/vnd.ms-excel", fileName);
        }       

        private ResultValidatedViewModel GetResultValidated(IEnumerable<ProblemAccountValidatedModel> validatedModels)
        {
            var result = new ResultValidatedViewModel
            {
                ProblemAccountInvalidList = validatedModels
                                                    .Where(x => !string.IsNullOrEmpty(x.ErrorMsg) && x.ErrorMsg != Consts.ProblemAccountManagement_DuplicatedUserName)
                                                    .Select(x => new ProblemAccountValidatedViewModel
                                                    {
                                                        UserName = x.UserName,
                                                        ErrMsg = x.ErrorMsg
                                                    }),
                IsCreditSite = !(validatedModels.Any(x => x.IsLicensee.HasValue ? x.IsLicensee.Value : false)),
                IsExistedSMA = validatedModels.Any(x => x.RoleID > 1)
            };

            return result;
        }

    }
}
