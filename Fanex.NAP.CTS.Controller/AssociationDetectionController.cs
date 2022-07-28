namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.Helpers;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.AssociationDetection;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.AssociationDetection;
    using Fanex.NAP.CTS.ViewModel.Common;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.AssociationDetection, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class AssociationDetectionController : BaseController
    {
        private readonly ICommonService _commonService;
        private readonly IAssociationDetectionService _associationDetectionService;
        private readonly ICustomerClassificationService _custClassificationService;

        public AssociationDetectionController()
        {
            _commonService = new CommonService();

            _associationDetectionService = new AssociationDetectionService();
            _custClassificationService = new CustomerClassificationService();
        }

        public ActionResult Index()
        {
            return View();
        }

        #region Old code

        //public ActionResult ValidateUsernames(string userNames)
        //{
        //    var viewModel = ValidatedAccount(userNames);

        //    return Json(viewModel, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //[TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociationDetection)]
        //public ActionResult GetAssociationDetection(string userNames)
        //{
        //    var result = new GetAssociationDetectionViewModel();
        //    var groupLevel2ByUsername = Enumerable.Empty<GetAssociationDetectionModel>();
        //    var latestCustomerClassification = Enumerable.Empty<LatestClassificationModel>();
        //    var isException = false;

        //    if (!string.IsNullOrWhiteSpace(userNames))
        //    {
        //        var userNameList = userNames.Split(',');

        //        var newUserNames = string.Join(",", userNameList.Distinct());

        //        var validatedList = _commonService.ValidateUsernames(newUserNames, out isException);

        //        var custIds = string.Join(",", validatedList.Select(x => x.CustID));

        //        var ctsCustIds = string.Join(",", validatedList.Select(x => x.CTSCustID));

        //        try
        //        {
        //            var getAssDetectionFromMySQL = AppSettings.Get("GetAssDetectionFromMySQL") == "1";

        //            groupLevel2ByUsername = _associationDetectionService.GetAssociationDetection(ctsCustIds, getAssDetectionFromMySQL);

        //            latestCustomerClassification = _custClassificationService.GetLatestClassification(custIds);

        //            result.ItemList = TransformLatestCustomerClassification(groupLevel2ByUsername, latestCustomerClassification);
        //        }
        //        catch (Exception ex)
        //        {
        //            result.ErrorCode = 1;
        //            result.ErrorMessage = "Timeout Issue.Please try again";

        //            var logMessage = new LogMessage(ex.Message)
        //               .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
        //               .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

        //            LogService.Error($"GetAssociationDetection exeption message:  {logMessage}");
        //        }

        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}        

        //private IEnumerable<AssociationDetectionItemViewModel> TransformLatestCustomerClassification(IEnumerable<GetAssociationDetectionModel> groupLevel2ByUsername
        //    , IEnumerable<LatestClassificationModel> latestCustomerClassification)
        //{
        //    if (groupLevel2ByUsername != null && groupLevel2ByUsername.Any())
        //    {
        //        return groupLevel2ByUsername.Select(x =>
        //        {
        //            var customerClass = x.CustSubId == 0 && x.CustomerCategory != null 
        //            ? latestCustomerClassification.FirstOrDefault(a => a.CustId == x.CustId)?.CustomerClass 
        //            : 0;

        //            return new AssociationDetectionItemViewModel
        //            {
        //                GroupId = x.GroupId,
        //                UserName = x.UserName,
        //                CustomerCategory = x.CustomerCategory,
        //                CategoryIds = x.CategoryIds,
        //                Evidence = x.Evidence,
        //                Status = x.CustStatus,
        //                CustomerClass = customerClass > 0 ? customerClass.ToString() : null,
        //                IsProblemAccount = x.ParentIds != null && x.ParentIds.Split(',').Any(y => y == "50"),
        //                IsPAProbation = x.IsProbation
        //            };
        //        });
        //    }

        //    return new List<AssociationDetectionItemViewModel>();
        //}

        //private ValidatedAccountsViewModel ValidatedAccount(string userNames)
        //{
        //    var isException = false;
        //    var viewModel = new ValidatedAccountsViewModel { IsException = false };

        //    if (!string.IsNullOrEmpty(userNames))
        //    {
        //        var userNameList = userNames.Split(',');
        //        var userNameToUpperList = userNameList.Select(x => x.ToUpper());

        //        var duplicateToUpperList = userNameToUpperList.GroupBy(x => x)
        //               .Where(g => g.Count() > 1)
        //               .Select(x => x.Last());

        //        var noDuplicateToUpperList = userNameToUpperList.Distinct();
        //        var noDuplicateToUpperUserName = string.Join(",", noDuplicateToUpperList);

        //        var validatedList = _commonService.ValidateUsernames(noDuplicateToUpperUserName, out isException);
        //        var validatedToUpperList = validatedList.Select(x => new ValidateUserNameModel
        //        {
        //            CTSCustID = x.CTSCustID,
        //            CustID = x.CustID,
        //            UserName = x.UserName.ToUpper()
        //        });

        //        viewModel = GenerateValidatedViewModel(userNames, validatedToUpperList, isException);

        //        viewModel.UserNameDuplicatedList = userNameList.Where(x => duplicateToUpperList.Contains(x.ToUpper()));
        //        viewModel.UserNameValidList = validatedList.Select(x => x.UserName);
        //    }

        //    return viewModel;
        //}

        //private ValidatedAccountsViewModel GenerateValidatedViewModel(string userNames, IEnumerable<ValidateUserNameModel> validatedToUpperList, bool isException)
        //{
        //    var userNameList = userNames.Split(',');
        //    var invalidList = Enumerable.Empty<string>();

        //    if (!isException)
        //    {
        //        var userNameToUpperValidatedList = validatedToUpperList.Select(x => x.UserName.ToUpper());

        //        invalidList = userNameList.Where(x => !userNameToUpperValidatedList.Contains(x.ToUpper()));
        //    }
        //    else
        //        invalidList = userNameList;

        //    return new ValidatedAccountsViewModel
        //    {
        //        IsException = isException,
        //        UserNameInvalidList = invalidList
        //    };
        //}

        #endregion

        public ActionResult ValidateUsernames(string userNames)
        {
            var isException = false;
            var invalidList = Enumerable.Empty<string>();
            var viewModel = new ValidatedAccountsViewModel { IsException = false };

            if (!string.IsNullOrEmpty(userNames))
            {
                var userNameList = userNames.Split(',');
                var userNameToUpperList = string.Join(",", userNameList.Select(x => x.ToUpper()).Distinct());             

                var validatedList = _commonService.ValidateUsernames(userNameToUpperList, out isException);

                var validatedUserNameList = validatedList.Select(x => x.UserName.ToUpper());               
                
                if (isException)
                {
                    invalidList = userNameList;
                }
                else
                {                    
                    invalidList = userNameList.Where(x => !validatedUserNameList.Contains(x.ToUpper()));
                }
               
                viewModel.IsException = isException;
                viewModel.UserNameInvalidList = invalidList;
                viewModel.UserNameValidList = validatedList.Select(x => x.UserName);
            }

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }
       
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociationDetection)]
        public ActionResult DetectAssociationByUserNames(DetectAssociationByUserNamesViewModel model)
        {
            var result = new DetectAssociationViewModel();
            var isException = false;

            if (!string.IsNullOrWhiteSpace(model.UserNames))
            {
                var userNameList = model.UserNames.Split(',');

                var validatedList = _commonService.ValidateUsernames(string.Join(",", userNameList.Distinct()), out isException);

                var custIds = string.Join(",", validatedList.Select(x => x.CustID));    

                try
                {
                    if (model.IsIPLast3Days)
                    {
                        var detectAssByIP = _associationDetectionService.DetectAssociationByIP(custIds);

                        model.SharedIPJson = JsonConvert.SerializeObject(detectAssByIP);
                    }

                    if (model.Is3MatchesLast7Days)
                    {
                        var detectAssByAssMatches = _associationDetectionService.DetectAssociationBySharedMatches(custIds);

                        model.SharedMatchesJson = JsonConvert.SerializeObject(detectAssByAssMatches);
                    }
                 
                    var detectAssByBPDeviceIPAgent = _associationDetectionService.DetectAssociationByBPDeviceIPAgent(model);
                   
                    // Merge Result

                    result.ItemList = new List<DetectAssociationItemViewModel>
                    {
                        new DetectAssociationItemViewModel
                        {
                            UserName = "Test",
                            CTSCustID = 1,
                            CustID = 1,
                            CategoryIDs = "53,45",
                            CustStatus = "Open",
                            Evidences = "6.2",
                            AssociationTypes = "IP, IP(last3days)",
                            CustomerCategory = "Hedging(Losing)",
                            CustomerClass = 106
                        }
                    };

                    /* ... */
                }
                catch (Exception ex)
                {
                    result.ErrorCode = 1;
                    result.ErrorMessage = "Timeout Issue.Please try again";

                    var logMessage = new LogMessage(ex.Message)
                       .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error(logMessage);
                }
            }            
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private string DetectAssociationByIP(string custIDList)
        {
            return "";
        }

        private string DetectAssociationBySharedMatches(string custIDList)
        {
            return "";
        }
    }
}
