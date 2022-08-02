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
    using Fanex.NAP.CTS.Resource.Consts;
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
        private const string AssociationDetectionTemplate = "~/App_Data/Excel Template/AssociationDetection/Template.xlsx";
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
            return Json(new
            {
                data = DetectAssociation(model),
                //sharedIPJson = model.SharedIPJson,
                sharedIPJson = GenerateSharedIPJson(),
                //sharedMatchesJson = model.SharedMatchesJson,
                sharedMatchesJson = GenerateSharedMatches()
            }, JsonRequestBehavior.AllowGet);
        }

        private string GenerateSharedIPJson()
        {
            var sharedIPs = new List<SharedIPMatchesModel>()
            {
                new SharedIPMatchesModel {
                    FromCustID = 1,
                    FromCTSCustID = 2,
                    FromUserName = "UserName 1",
                    ToCustID = 10,
                    ToCTSCustID = 20,
                    ToUserName = "UserName 2"
                },
                new SharedIPMatchesModel {
                    FromCustID = 1,
                    FromCTSCustID = 2,
                    FromUserName = "UserName 1",
                    ToCustID = 101,
                    ToCTSCustID = 202,
                    ToUserName = "UserName 3"
                }
            };

            return JsonConvert.SerializeObject(sharedIPs);
        }

        private string GenerateSharedMatches()
        {
            var sharedMatches = new List<SharedIPMatchesModel>()
            {
                new SharedIPMatchesModel {
                    FromCustID = 21061114,
                    FromCTSCustID = 119870,
                    FromUserName = "NextBetRMB01001",
                    ToCustID = 21061121,
                    ToCTSCustID = 119872,
                    ToUserName = "NextBetWon01001"
                },
                new SharedIPMatchesModel {
                    FromCustID = 12035060,
                    FromCTSCustID = 74125,
                    FromUserName = "12BETRM04094",
                    ToCustID = 21061114,
                    ToCTSCustID = 119870,
                    ToUserName = "NextBetRMB01001"
                }
            };

            return JsonConvert.SerializeObject(sharedMatches);
        }
      
        private DetectAssociationViewModel DetectAssociation(DetectAssociationByUserNamesViewModel model)
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
                    model.SharedIPJson = DetectAssociationByIP(validatedList, model.IsIPLast3Days, custIds);

                    model.SharedMatchesJson = DetectAssociationBySharedMatches(validatedList, model.Is3MatchesLast7Days, custIds);

                    var detectAss = _associationDetectionService.DetectAssociationByBPDeviceIPAgent(model);

                    var groups = detectAss.GroupBy(p => p.GroupID,(key, g) => new { GroupID = key, CTSCustIDs = g.Select(x => x.CTSCustID) });

                    result.ItemList = detectAss.Select(x =>
                    {                        
                        var item = new DetectAssociationItemViewModel(x);
                        var ctsCustIDs = groups.FirstOrDefault(y => y.GroupID == x.GroupID);
                        item.CTSCustIDList = ctsCustIDs != null ? string.Join(",", ctsCustIDs.CTSCustIDs) : string.Empty;

                        return item;
                    });
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

            return result;
        }

        private void GroupCTSCustID(IEnumerable<DetectAssociationByBPDeviceIPAgentModel> data)
        {          
            var results = data.GroupBy(
                p => p.GroupID,
                (key, g) => new { GroupID = key, CTSCustIDs = g.Select(x => x.CTSCustID) });
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociationDetection)]
        public ActionResult GetDetectAssociationDetail(GetAssociationDetectionDetailViewModel model)
        {
            var result = new List<GetAssociationDetectDetailViewModel>();

            var data = _associationDetectionService.GetAssociationDetectionDetail(model);

            result.AddRange(GetAssociationDetectSharedIP(model.CustID, model.SharedIPJson));

            result.AddRange(GetAssociationDetectSharedMatches(model.CustID, model.SharedMatchesJson));

            if (data != null && data.Any())
            {
                /* ... */
            }
            else
            {
                result = new List<GetAssociationDetectDetailViewModel>()
                {
                    new GetAssociationDetectDetailViewModel
                    {
                         UserName = "Member 03",
                         CustID = 2,
                         CTSCustID = 2,
                         AssociatedTypes = "<span class='association-detect-detail-agent'>Agent</span>"
                    },
                    new GetAssociationDetectDetailViewModel
                    {
                         UserName = "Member 05",
                         CustID = 3,
                         CTSCustID = 3,
                         AssociatedTypes = "Betting Pattern"
                    }
                };
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<GetAssociationDetectDetailViewModel> GetAssociationDetectSharedIP(int custID, string sharedIPJson)
        {
            if (!string.IsNullOrEmpty(sharedIPJson))
            {
                var sharedIPList = JsonConvert.DeserializeObject<IEnumerable<SharedIPMatchesModel>>(sharedIPJson);

                var sharedIPByCust = sharedIPList.Where(x => x.FromCustID == custID || x.ToCustID == custID);

                if (sharedIPByCust.Any())
                {
                    return sharedIPByCust.Select(x => new GetAssociationDetectDetailViewModel
                    {
                        UserName = x.ToUserName,
                        CustID = x.ToCustID,
                        CTSCustID = x.ToCTSCustID,
                        AssociatedTypes = "<span class='association-detect-detail-ip'>IP(last3days)</span>"
                    });
                }              
            }

            return new List<GetAssociationDetectDetailViewModel>()
            {
                new GetAssociationDetectDetailViewModel
                {
                    UserName = "Member 02",
                    CustID = 2,
                    CTSCustID = 2,
                    AssociatedTypes = "<span class='association-detect-detail-ip'>IP(last3days)</span>"
                }
            };
        }

        private IEnumerable<GetAssociationDetectDetailViewModel> GetAssociationDetectSharedMatches(int custID, string sharedMatchesJson)
        {
            if (!string.IsNullOrEmpty(sharedMatchesJson))
            {
                var sharedMatchesList = JsonConvert.DeserializeObject<IEnumerable<SharedIPMatchesModel>>(sharedMatchesJson);

                var sharedMatchesByCust = sharedMatchesList.Where(x => x.FromCustID == custID || x.ToCustID == custID);

                if (sharedMatchesByCust.Any())
                {
                    return sharedMatchesByCust.Select(x => new GetAssociationDetectDetailViewModel
                    {
                        UserName = x.ToUserName,
                        CustID = x.ToCustID,
                        CTSCustID = x.ToCTSCustID,
                        AssociatedTypes = "<span class='association-detect-detail-match'>Match</span>"
                    });
                }
            }

            return new List<GetAssociationDetectDetailViewModel>() {
                new GetAssociationDetectDetailViewModel
                {
                    UserName = "Member 04",
                    CustID = 2,
                    CTSCustID = 2,
                    AssociatedTypes = "<span class='association-detect-detail-match'>Match</span>"
                }
            };
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociationDetection)]
        public ActionResult GetDetectAssociationDetailIP(int custID1, int custID2)
        {
            var result = Enumerable.Empty<GetAssociationDetectDetailIPViewModel>();

            var coupleSharedIP =_associationDetectionService.GetCoupleSharedIP(custID1, custID2);

            if (coupleSharedIP != null && coupleSharedIP.Any())
            {
                result = coupleSharedIP.Select(x => new GetAssociationDetectDetailIPViewModel
                {
                    IP = x.IP
                });
            }           

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociationDetection)]
        public ActionResult GetDetectAssociationDetailMatch(int custID1, int custID2)
        {
            var result = Enumerable.Empty<GetAssociationDetectDetailMatchViewModel>();
            var custIDList = $"{custID1},{custID2}";

            var data = _associationDetectionService.GetDetectAssociationDetailMatch(custIDList);

            if (data != null && data.Any())
            {
                result = data.Select(x => new GetAssociationDetectDetailMatchViewModel(x));
            }
            else
            {
                result = new List<GetAssociationDetectDetailMatchViewModel>
                {
                    new GetAssociationDetectDetailMatchViewModel
                    {
                         MatchName = "Sutherland Sharks vs Blacktown City Demons",
                         EventDate = " Jul 16, 2022",
                         LeagueName = "AUSTRALIA NEW SOUTH WALES PREMIER LEAGUE",
                         UserName = "Username01",
                         TransDate = DateTime.Now.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                         TransID = 115719773059,
                         BetTypeName = "Handicap",
                         BetTeam = "h",
                         Stake = 500,
                         Odd = new decimal(0.95),
                         LiveScore = "0-1",
                         Result = "Won"
                    }
                };
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociationDetection)]
        public ActionResult GetDetectAssociationDetailAgent()
        {            
            var result = new List<GetAssociationDetectDetailAgentViewModel>
            {
                new GetAssociationDetectDetailAgentViewModel 
                { 
                     UserName = "Member 01",
                     Agent = "Agent 01",
                     AssociatedType = "Device"
                },
                new GetAssociationDetectDetailAgentViewModel
                {
                    UserName = "Member 01",
                    Agent = "Agent 01",
                    AssociatedType = "Device"
                }
            };

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociationDetection)]
        public ActionResult ExportToExcelAssociationDetect(DetectAssociationByUserNamesViewModel model)
        {
            var templatePath = Server.MapPath(AssociationDetectionTemplate);
            var dateTime = DateTime.UtcNow.AddHours(-4);

            var fileName = string.Format("AssociationDetect_{0}_{1}.xlsx"            
                , dateTime.ToString(FormatDateTimeToString_YYYYmmdd)
                , dateTime.ToString(Consts.FormatTimeToString_hhmmss));

            var data = DetectAssociation(model);
          
            var excelFile = _associationDetectionService.ExportToExcelAssociationDetect(data.ItemList, fileName, templatePath);

            return File(excelFile, "application/vnd.ms-excel", fileName);
        }

        private string DetectAssociationByIP(IEnumerable<ValidateUserNameModel> validateUserNames, bool isIPLast3Days, string custIds)
        {            
            if (isIPLast3Days)
            {
                var detectAssByIP = _associationDetectionService.DetectAssociationByIP(custIds);

                return DetectAssociationBySharedIPMatches(detectAssByIP, validateUserNames);
            }

            return JsonConvert.SerializeObject(Enumerable.Empty<SharedIPMatchesModel>());
        }

        private string DetectAssociationBySharedMatches(IEnumerable<ValidateUserNameModel> validateUserNames, bool is3MatchesLast7Days, string custIds)
        {           
            if (is3MatchesLast7Days)
            {
                var detectAssByAssMatches = _associationDetectionService.DetectAssociationBySharedMatches(custIds);

                return DetectAssociationBySharedIPMatches(detectAssByAssMatches, validateUserNames);
            }

            return JsonConvert.SerializeObject(Enumerable.Empty<SharedIPMatchesModel>());
        }

        private string DetectAssociationBySharedIPMatches(IEnumerable<DetectAssociationBySharedIPMatchesModel> detectAss
            , IEnumerable<ValidateUserNameModel> validateUserNames)
        {            
            var detectModel = detectAss.Select(x =>
            {
                var fromCTSCust = validateUserNames.FirstOrDefault(y => y.CustID.Equals(x.CustID1)) ?? new ValidateUserNameModel();
                var toCTSCust = validateUserNames.FirstOrDefault(y => y.CustID.Equals(x.CustID2)) ?? new ValidateUserNameModel();

                return new SharedIPMatchesModel
                {

                    FromCustID = x.CustID1,
                    FromCTSCustID = fromCTSCust.CTSCustID,
                    FromUserName = fromCTSCust.UserName,
                    ToCustID = x.CustID2,
                    ToCTSCustID = toCTSCust.CTSCustID,
                    ToUserName = toCTSCust.UserName
                };
            }).Where(x => x.FromCTSCustID != 0 && x.ToCTSCustID != 0);

            return JsonConvert.SerializeObject(detectModel);
        }
        
    }
}
