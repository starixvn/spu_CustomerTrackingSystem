namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Logging;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
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
        private readonly IProblemAccountManagementService _problemAccountManagementService;

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public ProblemAccountManagementController()
        {
            _problemAccountManagementService = new ProblemAccountManagementService();
        }

        public ActionResult Index()
        {
            var viewModel = Enumerable.Empty<SubscriberGroupViewModel>();
            var model = _problemAccountManagementService.GetAllSites();

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

            var paModel = _problemAccountManagementService.GetProblemAccounts(filter);

            if (paModel != null && paModel.Any())
            {
                var paCommentSelModel = Enumerable.Empty<ProblemAccountCommentSelModel>();
                var custIdList = string.Join(",", paModel.Select(x => x.CustID).Distinct());

                if (!isExportExcel)
                {
                    paCommentSelModel = _problemAccountManagementService.GetProblemAccountCommentSel(custIdList);
                }                

                problemAccounts = paModel.OrderByDescending(x => x.CreatedDate).Select(x => new ProblemAccountViewModel
                {
                    UserName = x.UserName,
                    Site = x.Site,
                    Currency = x.Currency,
                    Category = x.CategoryName,
                    Comment = isExportExcel ? string.Empty 
                                            : paCommentSelModel.Any(y => y.Custid == x.CustID) ? paCommentSelModel.FirstOrDefault(y => y.Custid == x.CustID).Comment : string.Empty,
                    CreatedBy = x.CreatedBy,
                    CreatedDateStr = x.CreatedDate.ToString(Consts.FormatDateToString_MMMddyyyy)
                });
            }

            return problemAccounts;
        }

        [HttpGet]
        public ActionResult GetProblemAccountAllCategories()
        {
            var data = _problemAccountManagementService.GetProblemAccountCategories();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ValidateProblemAccountInput(ValidateProblemAccountInputViewModel model)
        {
            var result = new ResultValidatedViewModel();

            var validatedModels = _problemAccountManagementService.ValidateProblemAccounts(model);

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
        public ActionResult UpdateProblemAccount(ValidateProblemAccountInputViewModel model)
        {
            var result = SaveProblemAccount(model);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.ProblemAccountManagementMarkNew)]
        public ActionResult UnmarkProblemAccount(ValidateProblemAccountInputViewModel model)
        {
            var result = SaveProblemAccount(model);

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
            var excelFile = _problemAccountManagementService.ExportProblemAccount(problemAccounts, fileName, templatePath, filter);

            return File(excelFile, "application/vnd.ms-excel", fileName);
        }

        private ResultSaveProblemAccountViewModel SaveProblemAccount(ValidateProblemAccountInputViewModel model)
        {
            var result = new ResultSaveProblemAccountViewModel();
            var paResultSiteCredits = Enumerable.Empty<ResultProblemAccountAddCustomerModel>();
            var paResultSiteLicensees = Enumerable.Empty<ResultProblemAccountAddCustomerModel>();

            var validatedModels = _problemAccountManagementService.ValidateProblemAccounts(model);

            if (validatedModels != null && validatedModels.Any())
            {
                var resultValidate = GetResultValidated(validatedModels);

                if (!resultValidate.ProblemAccountDuplicateList.Any()
                    && !resultValidate.ProblemAccountInvalidList.Any())
                {
                    var paForSiteCredit = validatedModels.Where(x => x.Site == Consts.ProblemAccountManagement_SiteCredit);
                    var paForSiteLicensee = validatedModels.Where(x => x.Site == Consts.ProblemAccountManagement_SiteLicensee);

                    switch (model.PAAction)
                    {
                        case ProblemAccountAction.Mark:
                            paResultSiteCredits = _problemAccountManagementService.MarkProblemAccountSiteCredit(paForSiteCredit, CurrentSession.UserId);

                            paResultSiteLicensees = _problemAccountManagementService.MarkProblemAccountSiteLicensee(paForSiteLicensee, CurrentSession.UserId);

                            break;

                        case ProblemAccountAction.Update:
                            paResultSiteCredits = _problemAccountManagementService.UpdateProblemAccountSiteCredit(paForSiteCredit, CurrentSession.UserId);

                            paResultSiteLicensees = _problemAccountManagementService.UpdateProblemAccountSiteLicensee(paForSiteLicensee, CurrentSession.UserId);                            

                            break;

                        case ProblemAccountAction.Unmark:
                            paResultSiteCredits = _problemAccountManagementService.UnmarkProblemAccountSiteCredit(paForSiteCredit, model.IsExceptDownlinesByTVS, model.IsExceptDownlinesByOPS, CurrentSession.UserId);

                            paResultSiteLicensees = _problemAccountManagementService.UnmarkProblemAccountSiteLicensee(paForSiteLicensee, CurrentSession.UserId);

                            break;
                    }
                }
                else
                {
                    return new ResultSaveProblemAccountViewModel { ErrorCode = -1, ResultValidated = resultValidate };
                }

                paResultSiteCredits = paResultSiteCredits.Union(paResultSiteLicensees);

                if (paResultSiteCredits.Any())
                {
                    result.ErrorCode = paResultSiteCredits.ToList()[0].ErrCode;
                    result.ErrorMessage = string.Join(",", paResultSiteCredits.Select(x => x.ErrMsg));
                    result.ResultValidated = resultValidate;
                }
            }

            return result;
        }

        private ResultValidatedViewModel GetResultValidated(IEnumerable<ProblemAccountValidatedModel> validatedModels)
        {
            var result = new ResultValidatedViewModel
            {
                ProblemAccountDuplicateList = validatedModels
                                                    .Where(x => x.ErrorMsg == Consts.ProblemAccountManagement_DuplicatedUserName)
                                                    .Select(x => new ProblemAccountValidatedViewModel
                                                    {
                                                        UserName = x.UserName,
                                                        ErrMsg = x.ErrorMsg
                                                    }),
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
