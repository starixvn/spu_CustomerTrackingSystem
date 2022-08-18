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
    using System.Threading.Tasks;
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
                data = DetectAssociation(model, false)                
            }, JsonRequestBehavior.AllowGet);
        }      
      
        private DetectAssociationViewModel DetectAssociation(DetectAssociationByUserNamesViewModel model, bool isExportExcel)
        {
            var result = new DetectAssociationViewModel();
            var isException = false;

            if (!string.IsNullOrWhiteSpace(model.UserNames))
            {
                try
                {                    
                    var userNames = model.UserNames.Split(',');
                    var userNameList = string.Join(",", userNames.Distinct());

                    var orgUserList = userNames.Select(x => new ValidateUserNameModel
                    {
                        UserName = x
                    });

                    var validatedList = _commonService.ValidateUsernames(userNameList, out isException);

                    orgUserList = orgUserList.Join(validatedList, c1 => c1.UserName, c2 => c2.UserName,
                        (c1, c2) => new ValidateUserNameModel
                        {
                            UserName = c1.UserName,
                            CustID = c2.CustID,
                            CTSCustID = c2.CTSCustID,
                            IsInternal = c2.IsInternal
                        });
                 
                    var custIDs = string.Join(",", orgUserList.Select(x => x.CustID).Distinct());

                    var ctsCustIDs = string.Join(",", orgUserList.Select(x => x.CTSCustID));

                    var mergeGroupInfo = new MergeGroupInfoModel { CTSCustIDs = ctsCustIDs };
                   
                    var assBySharedIP = Enumerable.Empty<SharedIPMatchesInfoModel>();

                    Parallel.Invoke(() => {                      
                        assBySharedIP = model.IsIPLast3Days ? DetectAssociationByIP(orgUserList.Where(x => !x.IsInternal), custIDs) : Enumerable.Empty<SharedIPMatchesInfoModel>();

                        mergeGroupInfo.AssociationByIPJson = GetAssociationCust(assBySharedIP);            
                    }, () => {
                        LogService.Info($"Detect Associatied by Matches start time {DateTime.Now}");

                        var assBySharedMatches = model.Is3MatchesLast7Days ? DetectAssociationBySharedMatches(orgUserList.Where(x => !x.IsInternal), custIDs) : Enumerable.Empty<SharedIPMatchesInfoModel>();

                        mergeGroupInfo.AssociationByMatchesJson = GetAssociationCust(assBySharedMatches);                        

                        LogService.Info($"Detect Associatied by Matches end time {DateTime.Now}");
                    }, () => {                        
                        var assByPrimary = model.IsBPManuallyDeviceIP ? _associationDetectionService.DetectAssociationByPrimary(ctsCustIDs) : Enumerable.Empty<DetectAssociationByPrimaryModel>();

                        mergeGroupInfo.AssociationByPrimaryJson = JsonConvert.SerializeObject(assByPrimary);                       
                    }, () => {                      
                        var assByAgent = model.IsAgent ? _associationDetectionService.DetectAssociationByAgent(ctsCustIDs) : Enumerable.Empty<DetectAssociationByAgentModel>();

                        mergeGroupInfo.AssociationByAgentJson = JsonConvert.SerializeObject(assByAgent);                        
                    });

                    //LogService.Info($"Detect Associatied Merge Group start time {DateTime.Now}");
                    var assMergeGroup = _associationDetectionService.MergeGroupDetectAssociation(mergeGroupInfo);
                    //LogService.Info($"Detect Associatied Merge Group end time {DateTime.Now}");

                    if (isExportExcel)
                    {     
                        result.ExportItemList = orgUserList.Select(x => 
                        {                           
                            return new ExportAssociationDetectModel
                            {
                                UserName = x.UserName,
                                GroupID = assMergeGroup.FirstOrDefault(y => y.CustID == x.CustID).GroupID ?? null
                            };
                        });
                    }
                    else
                    {
                        result.ItemList = assMergeGroup.Where(x => x.GroupID.HasValue).OrderBy(x => x.GroupID).Select(x =>
                        {
                            var userNameBySharedIPs = new List<string>();

                            userNameBySharedIPs.AddRange(assBySharedIP.Where(y => y.FromCustID == x.CustID).Select(y => y.ToUserName));

                            userNameBySharedIPs.AddRange(assBySharedIP.Where(y => y.ToCustID == x.CustID).Select(y => y.FromUserName));

                            return new DetectAssociationItemViewModel(x, string.Join(",", userNameBySharedIPs.Distinct()));
                        });
                    }

                    LogService.Info($"Detect Associatied end time {DateTime.Now}");
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

        private IEnumerable<SharedIPMatchesInfoModel> DetectAssociationByIP(IEnumerable<ValidateUserNameModel> validateUserNames, string custIds)
        {
            var detectAssByIP = _associationDetectionService.DetectAssociationByIP(custIds);

            return DetectAssociationBySharedIPMatches(detectAssByIP, validateUserNames);
        }

        private IEnumerable<SharedIPMatchesInfoModel> DetectAssociationBySharedMatches(IEnumerable<ValidateUserNameModel> validateUserNames, string custIds)
        {
            var detectAssByAssMatches = _associationDetectionService.DetectAssociationBySharedMatches(custIds);

            return DetectAssociationBySharedIPMatches(detectAssByAssMatches, validateUserNames);
        }

        private IEnumerable<SharedIPMatchesInfoModel> DetectAssociationBySharedIPMatches(IEnumerable<DetectAssociationBySharedIPMatchesModel> detectAss
           , IEnumerable<ValidateUserNameModel> validateUserNames)
        {
            return detectAss.Select(x =>
            {
                var fromCTSCust = validateUserNames.FirstOrDefault(y => y.CustID.Equals(x.CustID1)) ?? new ValidateUserNameModel();
                var toCTSCust = validateUserNames.FirstOrDefault(y => y.CustID.Equals(x.CustID2)) ?? new ValidateUserNameModel();

                return new SharedIPMatchesInfoModel
                {
                    FromCustID = x.CustID1,
                    FromCTSCustID = fromCTSCust.CTSCustID,
                    FromUserName = fromCTSCust.UserName,
                    ToCustID = x.CustID2,
                    ToCTSCustID = toCTSCust.CTSCustID,
                    ToUserName = toCTSCust.UserName
                };
            }).Where(x => x.FromCTSCustID != 0 && x.ToCTSCustID != 0);
        }

        private string GetAssociationCust(IEnumerable<SharedIPMatchesInfoModel> ass)
        {
            var assCustBySharedIP = ass.Select(x => new AssociationCustModel
            {
                FromCTSCustID = x.FromCTSCustID,
                ToCTSCustID = x.ToCTSCustID
            });

            return JsonConvert.SerializeObject(assCustBySharedIP);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociationDetectionDetails)]
        public ActionResult GetDetectAssociationDetail(GetAssociationDetectionDetailViewModel model)
        {
            var result = new List<GetAssociationDetectDetailViewModel>();

            result.AddRange(GetAssociationDetectSharedIP(model.UserName, model.CustID, model.CustSharedByIPs));

            result.AddRange(GetAssociationDetectSharedMatches(model));

            var data = _associationDetectionService.GetAssociationDetectionDetail(model);
            
            if (data != null && data.Any())
            {
                foreach (var item in data)
                {
                    result.Add(new GetAssociationDetectDetailViewModel(item));
                }                          
            }
            
            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<GetAssociationDetectDetailViewModel> GetAssociationDetectSharedIP(string userName, int custID, string custSharedByIPs)
        {
            if (!string.IsNullOrEmpty(custSharedByIPs))
            {
                var sharedIPList = custSharedByIPs.Split(',').AsEnumerable();

                return sharedIPList.Select(x =>
                {
                    return new GetAssociationDetectDetailViewModel
                    {
                        UserName = x.ToString(),                       
                        AssociatedTypes = $"<span class='association-detect-detail-ip' data-fromusername={userName} data-fromcustid={custID}>IP(last3days)</span>"
                    };
                });               
            }

            return Enumerable.Empty<GetAssociationDetectDetailViewModel>();
        }

        private IEnumerable<GetAssociationDetectDetailViewModel> GetAssociationDetectSharedMatches(GetAssociationDetectionDetailViewModel model)
        {
            if (!string.IsNullOrEmpty(model.UserNameList))
            {
                var isException = false;
                var validatedList = _commonService.ValidateUsernames(model.UserNameList, out isException);

                var custIDList = string.Join(",", validatedList.Select(x => x.CustID));

                var custSharedMatches = _associationDetectionService.GetAssociationDetectSharedMatches(model.CustID, custIDList);

                if (custSharedMatches != null)
                {
                    return custSharedMatches.Distinct().Select(x =>
                    {      
                        return new GetAssociationDetectDetailViewModel
                        {
                            UserName = validatedList.FirstOrDefault(y => y.CustID == x).UserName ?? string.Empty,
                            CustID = (int)x,
                            AssociatedTypes = $"<span class='association-detect-detail-match' data-fromusername={model.UserName} data-fromcustid={model.CustID}>Match</span>"
                        };
                    });
                }
            }
            
            return Enumerable.Empty<GetAssociationDetectDetailViewModel>();
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociationDetectionDetails)]
        public ActionResult GetDetectAssociationDetailIP(string userNameList)
        {
            var result = new List<GetAssociationDetectDetailIPViewModel>();

            var isException = false;
            var validatedList = _commonService.ValidateUsernames(userNameList, out isException).ToList();

            if (validatedList.Count() > 1)
            {
                var coupleSharedIPs = _associationDetectionService.GetCoupleSharedIP(validatedList[0].CustID, validatedList[1].CustID);

                if (coupleSharedIPs != null && coupleSharedIPs.Any())
                {
                    foreach (var item in coupleSharedIPs)
                    {
                        if (!string.IsNullOrEmpty(item.IP))
                        {
                            var arrIPs = item.IP.Split(',').AsEnumerable().Select(x => new GetAssociationDetectDetailIPViewModel { IP = x });
                            result.AddRange(arrIPs);
                        }
                    }
                }
            }            

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociationDetectionDetails)]
        public ActionResult GetDetectAssociationDetailMatch(int custID1, int custID2)
        {
            var result = Enumerable.Empty<GetAssociationDetectDetailMatchViewModel>();
            var custIDList = $"{custID1},{custID2}";

            var tickets = _associationDetectionService.GetDetectAssociationDetailMatch(custIDList)
                .OrderByDescending(x=>x.MatchID)
                .ThenByDescending(x=>x.TransDate);

            if (tickets != null && tickets.Any())
            {
                result = tickets.Select(x => { return new GetAssociationDetectDetailMatchViewModel(x); });
            }
            
            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociationDetectionDetails)]
        public ActionResult GetDetectAssociationDetailAgent(string agentInfoJson)
        {
            var result = Enumerable.Empty<GetAssociationDetectDetailAgentViewModel>();

            if (!string.IsNullOrEmpty(agentInfoJson))
            {
                result = JsonConvert.DeserializeObject<IEnumerable<GetAssociationDetectDetailAgentViewModel>>(agentInfoJson);
            }
          
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

            var data = DetectAssociation(model, true);
          
            var excelFile = _associationDetectionService.ExportToExcelAssociationDetect(data.ExportItemList, fileName, templatePath);

            return File(excelFile, "application/vnd.ms-excel", fileName);
        }       
       
    }
}
