namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.SpecialCustomerClass;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.ViewModel.SpecialCustomerClass;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.SpecialCustomerClass, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class SpecialCustomerClassController : BaseController
    {
        private readonly ISpecialCustomerClassService _specialCCService;

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public SpecialCustomerClassController()
        {
            _specialCCService = new SpecialCustomerClassService();
        }

        #region public methods
        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.SpecialCustomerClass,
                            NAP.Core.Business.Constant.Policy.Host);

            ViewBag.CustomerCategorySportOrder = (from int item in Enum.GetValues(typeof(CustomerCategorySportType))
                                                  select new KeyValuePair<int, string>(item, ((CustomerCategorySportType)item).GetStringSlow()))
                                                 .OrderBy(x => CustomerCategorySportOrder[(CustomerCategorySportType)x.Key])
                                                 .ToList();

            return View(isHostPermission);
        }

        [HttpGet]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.SpecialCustomerClassRpt, "isSkipTracking")]
        public ActionResult GetCustomerClass(SearchSpecialCCParamsModel model, bool? isSkipTracking)
        {
            var result = Enumerable.Empty<CustomerClassViewModel>();
            try
            {
                result = _specialCCService.GetSpecialCustomerClass(model);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatus.InternalServerError;

                var logMessage = new LogMessage(ex.Message)
                   .AddMessage($"Special Customer Class Get Function Error: " + ex.Message + ex.StackTrace + "\n"
                    + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                    + ex.InnerException.StackTrace)
                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.SpecialCustomerClass, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.SpecialCustomerClassInsert)]
        public ActionResult AddCustomerClass(AddSpecialCCParamsModel paramsModel)
        {
            var result = new ValidateSpecialCCResultViewModel();
            try
            {
                paramsModel.UserList = SelectItemForUserList(paramsModel.UserList);
                var validateUsers = _specialCCService.ValidateSpecialCC(FunctionValidateSpecialCC.Add, paramsModel.UserList);

                if (validateUsers.Users.Any(x => !x.IsValid))
                {
                    result = validateUsers;
                }
                else
                {
                    _specialCCService.InsertCustomerClass(paramsModel, validateUsers, CurrentSession.UserId);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatus.InternalServerError;

                var logMessage = new LogMessage(ex.Message)
                   .AddMessage($"Special Customer Class Add Function Error: " + ex.Message + ex.StackTrace + "\n"
                    + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                    + ex.InnerException.StackTrace)
                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.SpecialCustomerClass, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.SpecialCustomerClassRemove)]
        public ActionResult RemoveCustomerClass(RemoveSpecialCCParamsModel paramsModel)
        {
            var result = new ValidateSpecialCCResultViewModel();

            try
            {
                paramsModel.UserList = SelectItemForUserList(paramsModel.UserList);
                var validateUsers = _specialCCService.ValidateSpecialCC(FunctionValidateSpecialCC.Remove, paramsModel.UserList);

                if (validateUsers.Users.Any(x => !x.IsValid))
                {
                    result = validateUsers;
                }
                else
                {
                    _specialCCService.RemoveCustomerClass(paramsModel, validateUsers, CurrentSession.UserId);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatus.InternalServerError;

                var logMessage = new LogMessage(ex.Message)
                   .AddMessage($"Special Customer Class Remove Function Error: " + ex.Message + ex.StackTrace + "\n"
                    + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                    + ex.InnerException.StackTrace)
                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region private methods
        private List<UserListSpecialCCParamsModel> SelectItemForUserList(List<UserListSpecialCCParamsModel> model)
        {
            if (model != null && model.Any())
            {
                return model.SelectMany(x =>
                {
                    return x.Username.Split(',').Select(y => new UserListSpecialCCParamsModel
                    {
                        Username = y,
                        SportID = x.SportID
                    });
                }).Distinct(x => new { x.Username, x.SportID }).ToList();
            }

            return model;
        }
        #endregion
    }
}
