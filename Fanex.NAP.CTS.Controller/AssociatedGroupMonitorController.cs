namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.AssociatedGroupMonitor;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.AssociatedGroupMonitor;
    using Fanex.NAP.CTS.ViewModel.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;   
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.AssociatedGroupMonitor, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class AssociatedGroupMonitorController : BaseController
    {
        private const string AssociatedAccountMonitorTemplate = "~/App_Data/Excel Template/AssociatedGroupMonitor/Template.xlsx";
        private readonly IAssociatedGroupMonitorService _associatedGroupService;
        private readonly IProblemAccountClassificationService _paClassificationService;
        private readonly ICommonService _commonService;

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public AssociatedGroupMonitorController()
        {
            _associatedGroupService = new AssociatedGroupMonitorService();
            _paClassificationService = new ProblemAccountClassificationService();
            _commonService = new CommonService();
        }

        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.AssociatedGroupMonitor,
                            NAP.Core.Business.Constant.Policy.Host);
                      
            return View(isHostPermission);
        }

        [HttpPost]
        public ActionResult GetAllSites()
        {
            var siteGroups = Enumerable.Empty<SiteGroupViewModel>();
            var sites = _commonService.GetSites(false);

            if (sites != null && sites.Any())
            {
                siteGroups = sites.GroupBy(g => g.SubscriberGroupName)
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
                         });               
            }

            return Json(siteGroups, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult GetAssociatedGroupMonitor()
        {
            var associatedGroups = Enumerable.Empty<AssociatedGroupMonitorViewModel>();

            var data = _associatedGroupService.GetAssociatedGroupMonitor();

            if (data != null && data.Any())
            {
                associatedGroups = data.Select(x => new AssociatedGroupMonitorViewModel(x));
            }           

            return Json(new { data = associatedGroups }, JsonRequestBehavior.AllowGet);
        }      

        #region Set Problem Account
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult GetProblemCategorySiteByGroup(long groupID, bool isLicensee)
        {
            var sites = _associatedGroupService.GetProblemCategorySites(groupID)
                .Where(x => x.IsLicensee == isLicensee && x.IsChosen)
                .Select(x => x.SiteName);

            var result = string.Join(", ", sites);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult GetProblemCategoryByGroupID(long groupID)
        {
            var result = new GetProblemCategoryByGroupIDViewModel();

            var data = _associatedGroupService.GetProblemCategoryByGroupID(groupID);

            if (data != null)
            {
                result = new GetProblemCategoryByGroupIDViewModel(data);

                var sites = _associatedGroupService.GetAssociatedGroupSites(groupID);

                result.AddSiteGroups(sites);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult SaveProblemCategoryByGroupID(SetProblemCategoryByGroupIDModel model)
        {
            model.ModifiedBy = CurrentSession.UserId;

            var result = _associatedGroupService.SetProblemCategoryByGroupID(model);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region New, Update, Remove Group

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedGroupMonitor, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult SaveAddAssociatedGroupMonitor(AddAssociatedGroupModel model)
        {
            model.CreatedBy = CurrentSession.UserId;

            var result = _associatedGroupService.AddAssociatedGroupMonitor(model);
         
            return Json(new
            {
                errorCode = result.ErrorCode,
                errorMessage = result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedGroupMonitor, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult UpdateAssociatedGroupMonitor(long groupID)
        {
            var result = new GetUpdateAssociatedGroupMonitorByIDViewModel();

            var data = _associatedGroupService.GetUpdateAssociatedGroupMonitorByID(groupID);

            if (data != null)
            {
                result = new GetUpdateAssociatedGroupMonitorByIDViewModel(data);

                var sites = _associatedGroupService.GetAssociatedGroupSites(groupID);

                if (sites != null && sites.Any())
                {
                    var siteGroups = sites.OrderBy(x=>x.IsLicensee)
                             .GroupBy(g => g.IsLicensee)
                             .Select(x => new { SubscriberGroupName = x.Key ? "Licensees" : "Credit", SiteList = x.ToList() })
                             .Select(x => new SiteGroupViewModel
                             {
                                 SubscriberGroupName = x.SubscriberGroupName,
                                 SiteList = x.SiteList.Select(y => new SiteViewModel
                                 {
                                     SiteID = y.SiteID,
                                     SiteName = y.SiteName,
                                     IsSelected = y.IsChosen
                                 })
                             });

                    result.AddSiteGroups(siteGroups);
                }                
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }      

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedGroupMonitor, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult SaveUpdateAssociatedGroupMonitor(UpdateAssociatedGroupMonitorModel model)
        {
            model.ModifiedBy = CurrentSession.UserId;

            var result = _associatedGroupService.UpdateAssociatedGroupMonitor(model);

            if (result.ErrorCode != -1)
            {
                result = _associatedGroupService.SetProblemCategoryByGroupID(new SetProblemCategoryByGroupIDModel
                {
                    GroupID = model.GroupID,                    
                    PACreditSites = model.PACreditSites,
                    PALicenseeSites = model.PALicenseeSites,
                    PACategoryID = -2,
                    ModifiedBy = model.ModifiedBy,
                });
            }

            return Json(new
            {
                errorCode = result.ErrorCode,
                errorMessage = result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedGroupMonitor, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult RemoveAssociatedGroupMonitor(RemoveAssociatedGroupModel model)
        {
            model.UserID = CurrentSession.UserId;

            var result = _associatedGroupService.RemoveAssociatedGroupMonitor(model);

            return Json(new
            {
                errorCode = result.ErrorCode,
                errorMessage = result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult GetSitesByGroup(long groupID, bool isLicensee)
        {
            var sites = _associatedGroupService.GetAssociatedGroupSites(groupID)
                .Where(x => x.IsLicensee == isLicensee && x.IsChosen)
                .Select(x => x.SiteName);

            var result = string.Join(", ", sites);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Add Account to Group

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedGroupMonitor, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult ValidateUserNameInput(string userNames)
        {
            return Json(GetResultValidated(userNames, 0), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedGroupMonitor, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult AddAssociatedAccount(AddAssociatedAccountModel model)
        {
            var result = new ResultSaveAssociatedAccountViewModel();
            
            var resultValidate = GetResultValidated(model.UserNames, 0);

            if (!resultValidate.AccountInvalidList.Any())
            {               
                model.CustIDs = resultValidate.CustIDs;
                model.CTSCustIDs = resultValidate.CTSCustIDs;
                model.CreatedBy = CurrentSession.UserId;

                var addResult = _associatedGroupService.AddAssociatedAccount(model);
                
                if (addResult.ErrorCode == 0)
                {
                    var custPas = addResult.CustInfos.Where(x => x.IsSyncPA).Select(x => new InsertProblemAccountModel {
                        CustID = x.CustID,
                        CTSCustID = x.CTSCustID,
                        RoleID = x.RoleID,
                        SubscriberID = x.SubscriberID,
                        IsLicensee = x.IsLicensee,
                        CategoryID = x.CategoryID,
                        CategoryGroup = x.CategoryGroup,                      
                        IsDirectPA = true,
                        IsFromCTS = true,
                        CreatedBy = CurrentSession.UserId,
                        Remark = Consts.MarkPA_AssociatedGroup + " - " + model.GroupName
                    });

                    if (custPas.Any())
                    {                        
                        _paClassificationService.InsertProblemAccountPackage(custPas, InsertPA_ByType.Category, "AssociatedGroupMonitorAddPA", FromFlow.CTSWEB, true);

                        var custGroupABIPA = custPas.Select(x => x.CustID).Distinct();
                        _associatedGroupService.UpdateABIToMainDB(string.Join(",", custGroupABIPA), model.ABI);
                    }

                    var custGroupABI = addResult.CustInfos.Where(x => !x.IsSyncPA).Select(x => x.CustID).Distinct();
                    if (custGroupABI.Any())
                    {
                        _associatedGroupService.UpdateABIToMainDB(string.Join(",", custGroupABI), model.ABI);
                    }

                    var custUpdateOri = addResult.CustInfos.Select(x => x.CustID).Distinct();
                    if (custUpdateOri.Any())
                    {
                        _associatedGroupService.UpdateOriToMainDB(string.Join(",", custUpdateOri), model.Ori);
                    }
                }

                result.ErrorCode = addResult.ErrorCode;
                result.ErrorMessage = addResult.ErrorMsg;
                result.ResultValidated = resultValidate;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private ResultValidatedViewModel GetResultValidated(string userNames, long groupID)
        {
            var result = new ResultValidatedViewModel();

            if (!string.IsNullOrEmpty(userNames))
            {
                var userNameList = userNames.Split(',');

                var userNamesDupIgnores = string.Join(",", userNameList.Distinct());

                var validatedModels = Enumerable.Empty<ValidateUserAssociatedAccountModel>();

                if (groupID == 0)
                    validatedModels = _associatedGroupService.ValidateUserNameInput(userNamesDupIgnores);
                else
                    validatedModels = _associatedGroupService.ValidateUsernameTransferAccount(userNamesDupIgnores, groupID);

                if (validatedModels != null && validatedModels.Any())
                {
                    result = new ResultValidatedViewModel
                    {
                        AccountInvalidList = validatedModels
                        .Where(x => !string.IsNullOrEmpty(x.ErrorMsg))
                        .Select(x => new ValidateUserAssociatedAccountViewModel
                        {
                            CTSCustID = x.CTSCustID,
                            UserName = x.Username,
                            ErrMsg = x.ErrorMsg                            
                        }),
                        CustIDs = string.Join(",", validatedModels.Select(x => x.CustID).Distinct()),
                        CTSCustIDs = string.Join(",", validatedModels.Select(x => x.CTSCustID).Distinct())
                    };                  
                }
            }
          
            return result;
        }

        #endregion

        #region Transfer Accounts
        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedGroupMonitor, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult GetGroupTransferAccount(long groupID)
        {
            var groups = _associatedGroupService.GetGroupTransferAccount(groupID);

            return Json(groups, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedGroupMonitor, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult ValidateUsernameTransferAccount(string userNames, long groupID)
        {
            return Json(GetResultValidated(userNames, groupID), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedGroupMonitor, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult SaveTransferAccount(SaveTransferAccountModel model)
        {
            model.ModifiedBy = CurrentSession.UserId;

            var result = new ResultSaveAssociatedAccountViewModel();

            var resultValidate = GetResultValidated(model.UserNames, model.FromGroupID);

            if (!resultValidate.AccountInvalidList.Any())
            {
                model.CTSCustIDs = resultValidate.CTSCustIDs;
                model.ModifiedBy = CurrentSession.UserId;

                var addResult = _associatedGroupService.SaveTransferAccount(model);

                result.ErrorCode = addResult.ErrorCode;
                result.ErrorMessage = addResult.ErrorMsg;
                result.ResultValidated = resultValidate;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Export to Excel
        [HttpPost]        
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult ExportToExcelAssociatedAccount(ExportToExcelAssociatedAccountViewModel model)
        {
            var templatePath = Server.MapPath(AssociatedAccountMonitorTemplate);
            var dateTime = DateTime.UtcNow.AddHours(-4);
            
            var fileName = string.Format("{0}_Group_{1}_{2}.xlsx"
                , model.GroupName
                , dateTime.ToString(FormatDateTimeToString_YYYYmmdd)
                , dateTime.ToString(Consts.FormatTimeToString_hhmmss));

            var associatedAccounts = _associatedGroupService.GetAssociatedAccountMonitor(model.GroupID);
           
            var sites = _associatedGroupService.GetAssociatedGroupSites(model.GroupID);

            model.IsAllSite = !sites.Where(x => !x.IsChosen).Any();

            if (!model.IsAllSite)
            {
                var creditSites = sites.Where(x => x.IsChosen).Where(x => !x.IsLicensee).Select(x => x.SiteName);
                var licenseeSites = sites.Where(x => x.IsChosen).Where(x => x.IsLicensee).Select(x => x.SiteName);

                model.CreditSites = string.Join(", ", creditSites);
                model.LicenseeSites = string.Join(", ", licenseeSites);
            }
                       
            var excelFile = _associatedGroupService.ExportExcelAssociatedAccount(associatedAccounts, fileName, templatePath, model);

            return File(excelFile, "application/vnd.ms-excel", fileName);
        }

        #endregion

    }
}
