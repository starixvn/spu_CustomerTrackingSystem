namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.AssociatedGroupMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.AssociatedGroupMonitor;
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

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public AssociatedGroupMonitorController()
        {
            _associatedGroupService = new AssociatedGroupMonitorService();
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
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult GetAssociatedGroupMonitor()
        {
            var associatedGroups = Enumerable.Empty<AssociatedGroupMonitorViewModel>();

            var data = _associatedGroupService.GetAssociatedGroupMonitor();

            if (data != null && data.Any())
            {
                associatedGroups = data.Select(x => new AssociatedGroupMonitorViewModel
                {
                    GroupID = x.GroupID,
                    GroupName = x.GroupName,
                    ABI = x.ABI,
                    TotalAccount = x.TotalAccount,
                    Remark = x.Remark,
                    CreatedDate = x.Created.ToString(Consts.FormatDateToString_MMMddyyyy),
                    CreatedBy = x.CreatedBy
                });
            }
           
            return Json(new { data = associatedGroups }, JsonRequestBehavior.AllowGet);
        }
      
        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedGroupMonitor, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult AddAssociatedGroupMonitor(AddAssociatedGroupViewModel model)
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
        public ActionResult RemoveAssociatedGroupMonitor(RemoveAssociatedGroupViewModel model)
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
        public ActionResult ValidateUserNameInput(string userNames)
        {           
            var validatedModels = _associatedGroupService.ValidateUserNameInput(userNames);
            
            return Json(this.GetResultValidated(validatedModels), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedGroupMonitor, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult AddAssociatedAccount(AddAssociatedAccountViewModel model)
        {
            var result = new ResultSaveAssociatedAccountViewModel();
            var validatedModels = _associatedGroupService.ValidateUserNameInput(model.UserNames);

            var resultValidate = GetResultValidated(validatedModels);

            if (!resultValidate.AccountInvalidList.Any())
            {               
                model.CustIDs = string.Join(",", validatedModels.Select(x => x.CustID).Distinct());
                model.CTSCustIDs = string.Join(",", validatedModels.Select(x => x.CTSCustID).Distinct());
                model.CreatedBy = CurrentSession.UserId;

                var dbResult = _associatedGroupService.AddAssociatedAccount(model);
                
                if (dbResult.ErrorCode == 0)
                {                    
                    _associatedGroupService.UpdateABIToMainDB(model.CustIDs, model.ABI);
                }

                result.ErrorCode = dbResult.ErrorCode;
                result.ErrorMessage = dbResult.ErrorMsg;
                result.ResultValidated = resultValidate;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private ResultValidatedViewModel GetResultValidated(IEnumerable<ValidateUserAssociatedAccountModel> validatedModels)
        {
            var result = new ResultValidatedViewModel();

            if (validatedModels != null && validatedModels.Any())
            {
                result.AccountInvalidList = validatedModels
                    .Where(x => !string.IsNullOrEmpty(x.ErrorMsg))
                    .Select(x => new ValidateUserAssociatedAccountViewModel
                    {
                        CTSCustID = x.CTSCustID,
                        UserName = x.Username,                       
                        ErrMsg = x.ErrorMsg
                    });
            }

            return result;
        }

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

            var excelFile = _associatedGroupService.ExportExcelAssociatedAccount(associatedAccounts, fileName, templatePath, model);

            return File(excelFile, "application/vnd.ms-excel", fileName);
        }

    }
}
