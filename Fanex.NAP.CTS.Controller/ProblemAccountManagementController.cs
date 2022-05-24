namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.ProblemAccountManagement;
    using Fanex.NAP.CTS.ViewModel.ProblemAccountMonitor;
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

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public ProblemAccountManagementController()
        {
            _pamService = new ProblemAccountManagementService();
            _paClassificationService = new ProblemAccountClassificationService();
        }

        public ActionResult Index()
        {
            var viewModel = Enumerable.Empty<SubscriberGroupViewModel>();
            var model = _pamService.GetAllSites();

            if (model != null)
            {
                viewModel = model.Where(x => x.IsActive)
                    .GroupBy(g => new { g.SubscriberGroupID, g.SubscriberGroupName })
                    .Select(x => new { Group = x.Key, SiteList = x.ToList() })
                    .Select(x => new SubscriberGroupViewModel
                    {
                        SubscriberGroupID = x.Group.SubscriberGroupID,
                        SubscriberGroupName = x.Group.SubscriberGroupName,
                        SiteList = x.SiteList.Select(y => new SiteViewModel
                        {
                            SiteID = y.SiteID,
                            SiteName = y.SiteName,
                            DisplayOrder = y.DisplayOrder
                        })
                    });
            }

            return View(viewModel);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.ProblemAccountManagementRpt)]
        public ActionResult GetProblemAccount(ProblemAccountFilterViewModel filter)
        {
            var problemAccounts = GetProblemAccounts(filter, false);

            return Json(new
            {
                data = problemAccounts
            }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<ProblemAccountViewModel> GetProblemAccounts(ProblemAccountFilterViewModel filter, bool isExportExcel)
        {
            var problemAccounts = Enumerable.Empty<ProblemAccountViewModel>();

            if (filter.ToDate != DateTime.MinValue)
            {
                filter.ToDate = new DateTime(filter.ToDate.Year, filter.ToDate.Month, filter.ToDate.Day, 23, 59, 59);
            }

            var paModel = _pamService.GetProblemAccounts(filter);

            if (paModel != null && paModel.Any())
            {
                problemAccounts = paModel.OrderByDescending(x => x.CreatedDate).Select(x => new ProblemAccountViewModel
                {
                    UserName = x.UserName,
                    Site = x.Site,
                    Currency = x.Currency,
                    Category = x.CategoryName,
                    Comment = x.Remark,
                    CreatedBy = x.CreatedBy,
                    CreatedDateStr = x.CreatedDate.ToString(Consts.FormatDateToString_MMMddyyyy)
                });
            }

            return problemAccounts;
        }

        [HttpGet]
        public ActionResult GetProblemAccountAllCategories()
        {
            var data = _pamService.GetProblemAccountCategories();

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
        public ActionResult MarkProblemAccount(ValidateProblemAccountInputViewModel model)
        {
            var result = SaveProblemAccount(model);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
       
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.ProblemAccountManagementMarkNew)]
        public ActionResult UnmarkProblemAccount(ValidateProblemAccountInputViewModel model)
        {
            var result = new ResultSaveProblemAccountViewModel();

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
          
            var problemAccounts = GetProblemAccounts(filter, true);            
            var excelFile = _pamService.ExportProblemAccount(problemAccounts, fileName, templatePath, filter);

            return File(excelFile, "application/vnd.ms-excel", fileName);
        }

        private ResultSaveProblemAccountViewModel SaveProblemAccount(ValidateProblemAccountInputViewModel models)
        {
            var result = new ResultSaveProblemAccountViewModel();

            var validatedModels = _pamService.ValidateProblemAccounts(models);

            if (validatedModels != null && validatedModels.Any())
            {
                var resultValidate = GetResultValidated(validatedModels);

                if (!resultValidate.ProblemAccountInvalidList.Any())
                {
                    var custIdList = validatedModels.Select(c => c.CustID)?.Distinct();
                    var custIds = string.Join(",", custIdList);

                    var pas = validatedModels.Select(x => new InsertProblemAccountModel
                    {
                        CustID = x.CustID,
                        CTSCustID = x.CTSCustID,
                        RoleID = x.RoleID,
                        SubscriberID = x.SubscriberID,
                        IsLicensee = x.IsLicensee,
                        CategoryID = models.CategoryID,
                        CategoryGroup = models.CategoryGroup,
                        Remark = models.Comment,
                        IsDirectPA = true,
                        IsFromPAM = true,
                        CreatedBy = CurrentSession.UserId
                    });

                    var insertResult = _paClassificationService.InsertProblemAccountPackage(pas, InsertPA_ByType.Category, "ProblemAccountManagement", FromFlow.CTSWEB);

                    result.ErrorCode = insertResult.ErrorCode;
                    result.ErrorMessage = insertResult.ErrorMsg;
                    result.ResultValidated = resultValidate;
                }
            }

            return result;
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
                IsCreditSite = validatedModels.Where(x=>x.Site == Consts.ProblemAccountManagement_SiteCredit).Any()
            };

            return result;
        }

    }
}
