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
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using Fanex.NAP.CTS.Model.SpecialCustomerClass;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
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
        private readonly IRescanCustomerClassificationService _customerClassifyService;
        private readonly ICustomerClassificationService _custClassificationService;
        private readonly IDataLogTrackingServices dataLogTrackingServices;

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public SpecialCustomerClassController()
        {
            _specialCCService = new SpecialCustomerClassService();
            _custClassificationService = new CustomerClassificationService();
            _customerClassifyService = new RescanCustomerClassificationService();
            this.dataLogTrackingServices = new DataLogTrackingServices();
        }

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
        public ActionResult GetCustomerClass(string userName, bool? isSkipTracking)
        {
            var viewModel = Enumerable.Empty<CustomerClassViewModel>();
            var specialCustomers = _specialCCService.GetAllCustomerClass(userName);

            if (specialCustomers != null && specialCustomers.Any())
            {
                viewModel = specialCustomers.Select(x => new CustomerClassViewModel
                {
                    CTSCustID = x.CTSCustID,
                    CustID = x.CustID,
                    RootCTSCustID = x.RootCTSCustID,
                    Username = x.Username,
                    SubscriberName = x.SubscriberName,
                    CustomerClass = x.CustomerClass,
                    CreatedBy = x.CreatedBy,
                    CreatedDateString = x.CreatedDate != DateTime.MinValue
                        ? x.CreatedDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss)
                        : string.Empty
                });
            }

            return Json(new { data = viewModel }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.SpecialCustomerClass, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.SpecialCustomerClassInsert)]
        public ActionResult AddCustomerClass(AddSpecialCCParamsModel paramsModel)
        {
            var result = new ValidateSpecialCCResultViewModel();
            try
            {
                var validateUsers = _specialCCService.ValidateSpecialCC(FunctionValidateSpecialCC.Add, paramsModel.UserList);

                if (validateUsers.Users.Any(x => !x.IsValid))
                {
                    result = validateUsers;
                    Response.StatusCode = (int)HttpStatus.BadRequest;
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
            //var resultInsert = new ResultInsertRemoveViewModel
            //{
            //    ErrorCode = 0,
            //    ErrorMessage = string.Empty
            //};

            //var resultInsertToCTS = _customerClassService.InsertCustomerClass(username, custClass, isAffectToAss, CurrentSession.UserId);

            //if (string.IsNullOrEmpty(resultInsertToCTS.OpErrorMessage))
            //{
            //    if (resultInsertToCTS.CustomerClassList != null && resultInsertToCTS.CustomerClassList.Any())
            //    {
            //        var custIdsChunk = resultInsertToCTS.CustomerClassList.Chunk(ConfigSettings.PushCCToExternalChunkSize);

            //        foreach (var custs in custIdsChunk)
            //        { 
            //            _custClassificationService.PushDataToExternal(custs.Select(c=>c.CustId).Distinct(), new PushDataToExternalModel
            //            {
            //                IsUpdateDangerToMainDB = false,
            //                IsSyncAFC = false,
            //                IsPushCCToSoon88 = false,
            //                ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize
            //            });
            //        }
            //    }
            //}
            //else
            //{
            //    resultInsert.ErrorCode = -1;
            //    resultInsert.ErrorMessage = resultInsertToCTS.OpErrorMessage;
            //}

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
                var validateUsers = _specialCCService.ValidateSpecialCC(FunctionValidateSpecialCC.Remove, paramsModel.UserList);

                if (validateUsers.Users.Any(x => !x.IsValid))
                {
                    result = validateUsers;
                    Response.StatusCode = (int)HttpStatus.BadRequest;
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
            //try
            //{
            //    var resultRemoveToCTS = _specialCCService.RemoveCustomerClass(ctsCustID, CurrentSession.UserId);

            //    if (resultRemoveToCTS.ErrorCode == 0)
            //    {
            //        _customerClassifyService.RescanCustomerClassification(new RescanCustClassificationModel
            //        {
            //            CustIds = new List<long> { custID },
            //            IsRescanPA = true,
            //            IsPushCCToSoon88 = true,
            //            IsUpdateDangerToMainDB = true,
            //            IsToQueue = false,
            //            FunctionId = FunctionId.RemoveSpecialCCWeb,
            //            ServiceName = "RemoveSpecialCustomer"
            //        });
            //    }

            //    return Json(resultRemoveToCTS, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception ex)
            //{
            //    LogService.Error($"CTS - RemoveSpecialCustomer exception: { ex.Message }", ex);

            //    dataLogTrackingServices.LogErrorData(ex, "WEB-RemoveSpecialCustomer");

            //    return Json(new { errorCode = -1, errorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            //}
        }
    }
}
