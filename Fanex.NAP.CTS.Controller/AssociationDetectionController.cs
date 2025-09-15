namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.AssociationDetection;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
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

        public AssociationDetectionController()
        {
            _commonService = new CommonService();
            _associationDetectionService = new AssociationDetectionService();
        }

        public ActionResult Index()
        {
            return View();
        }

        #region Validate Usernames

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
        #endregion

        #region Detect Association

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, null,
            "IsBP:AssociationDetectionByBettingPattern=65",
            "IsDevice:AssociationDetectionByDevice=66",
            "IsIP:AssociationDetectionByIP=67",
            "IsAgent:AssociationDetectionByAgent=68",
            "Is3MatchesLast7Days:AssociationDetectionByMatch7Days=69",
            "IsIPLast3Days:AssociationDetectionByIP3Days=70")]
        public ActionResult DetectAssociationByUserNames(DetectAssociationByUserNamesViewModel model)
        {
            var result = new DetectAssociationViewModel();

            try
            {
                if (!string.IsNullOrWhiteSpace(model.UserNames))
                {                    
                    result = ConfigSettings.IsUsingFanexAPIDirect 
                                ? ApiHelper.PostWithApiKey<DetectAssociationViewModel>(
                                               ConfigSettings.GetCtsApiEndPoint(ApiEndpoint.DetectAssociationByUserNames),
                                               model,
                                               ConfigSettings.ApiKeyFanexAPIDirect).Result
                                : DetectAssociation(model);
                  
                }
            }
            catch (Exception ex)
            {
                result.ErrorCode = 1;
                result.ErrorMessage = ex.Message;

                ex.LogError();
            }

            return Json(new
            {
                data = result,
                filters = GenerateFilter(result)
            }, JsonRequestBehavior.AllowGet);
        }

        private DetectAssociationLoadFilterViewModel GenerateFilter(DetectAssociationViewModel data)
        {
            var custClassList = _commonService.GetGroupCustomerClass().ToList();

            var CustClassFilter = data.ItemList.Select(c => c.CustomerClass).Distinct().ToList();

            var filters = new DetectAssociationLoadFilterViewModel(custClassList, CustClassFilter);

            #region Danger List
            filters.DangerFilter.Danger1 = data.ItemList.Where(x => x.Danger1 > 0).Select(x => x.Danger1).Distinct()
               .Select(x => new DetectAssociationFilterDangerViewModel { Danger = x, DangerName = "Original " + x })
               .OrderBy(x => x.Danger);

            filters.DangerFilter.Danger2 = data.ItemList.Where(x => x.Danger2 > 0).Select(x => x.Danger2).Distinct()
               .Select(x => new DetectAssociationFilterDangerViewModel { Danger = x, DangerName = "ABI " + x })
               .OrderBy(x => x.Danger);

            filters.DangerFilter.Danger3 = data.ItemList.Where(x => x.Danger3 > 0).Select(x => x.Danger3).Distinct()
                .Select(x => new DetectAssociationFilterDangerViewModel { Danger = x, DangerName = "Other Sports " + x })
                .OrderBy(x => x.Danger);

            filters.DangerFilter.Danger4 = data.ItemList.Where(x => x.Danger4 > 0).Select(x => x.Danger4).Distinct()
                .Select(x => new DetectAssociationFilterDangerViewModel { Danger = x, DangerName = "Basketball " + x })
                .OrderBy(x => x.Danger);

            filters.DangerFilter.Danger5 = data.ItemList.Where(x => x.Danger5 > 0).Select(x => x.Danger5).Distinct()
               .Select(x => new DetectAssociationFilterDangerViewModel { Danger = x, DangerName = "E-Sports " + x })
               .OrderBy(x => x.Danger);

            if (data.ItemList.Any(x => x.DangerDisplay == "-"))
            {
                filters.DangerFilter.NoneDanger = new List<DetectAssociationFilterDangerViewModel> { new DetectAssociationFilterDangerViewModel { Danger = 0, DangerName = "-" } };
            }
            #endregion

            return filters;
        }

        private DetectAssociationViewModel DetectAssociation(DetectAssociationByUserNamesViewModel model)
        {
            var result = new DetectAssociationViewModel();

            var orgUserList = _associationDetectionService.ValidateUserName(model.UserNames);

            var assMergeGroup = _associationDetectionService.DetectAssociation(model, orgUserList, true);

            var assMergeGroupHasGroupID = assMergeGroup?.Where(x => x.GroupID.HasValue);

            var assRearrangeGroup = RearrangeAssGroup(assMergeGroupHasGroupID);

            result.ItemList = assRearrangeGroup.Select(x =>
            {
                return new DetectAssociationItemViewModel(x);
            });

            return result;
        }

        private DetectAssociationViewModel PrepareDataExportDetectAssociation(DetectAssociationByUserNamesViewModel model)
        {
            var result = new DetectAssociationViewModel();

            if (!string.IsNullOrWhiteSpace(model.UserNames))
            {
                var orgUserList = _associationDetectionService.ValidateUserName(model.UserNames);

                var assMergeGroup = _associationDetectionService.DetectAssociation(model, orgUserList, true);

                var assRearrangeGroup = RearrangeAssGroup(assMergeGroup);

                var custIDs = string.Join(",", orgUserList.Select(x => x.CustID).Distinct());

                var custStatisticsList = _associationDetectionService.GetCustStatisticsInfoRealtime(custIDs);

                result.ExportItemList = GetExportItem(orgUserList, assRearrangeGroup, custStatisticsList);
            }

            return result;
        }

        private IEnumerable<DetectAssociationMergeGroupModel> RearrangeAssGroup(IEnumerable<DetectAssociationMergeGroupModel> assGroupList)
        {
            var result = new List<DetectAssociationMergeGroupModel>();

            if (assGroupList.IsNotNullOrEmpty())
            {
                var idx = 0;
                var groupIdDict = new Dictionary<string, int>();

                foreach (var item in assGroupList)
                {
                    var key = item.GroupID?.ToString() ?? string.Empty;
                    if (item.GroupID.HasValue && !groupIdDict.ContainsKey(item.GroupID?.ToString() ?? string.Empty))
                    {
                        idx++;
                        groupIdDict[key] = idx;
                    }

                    if (item.GroupID.HasValue)
                    {
                        item.GroupID = groupIdDict[key];
                    }
                }

                result.AddRange(assGroupList.OrderBy(x => x.GroupID));
            }

            return result;
        }

        #endregion

        #region Detail
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociationDetectionDetails)]
        public ActionResult GetDetectAssociationDetail(GetAssociationDetectionDetailViewModel model)
        {
            var result = Enumerable.Empty<GetAssociationDetectDetailViewModel>();

            var assBySharedIP = Enumerable.Empty<GetAssociationDetectDetailViewModel>();
            var assBySharedMatch = Enumerable.Empty<GetAssociationDetectDetailViewModel>();
            var assDetails = Enumerable.Empty<GetAssociationDetectDetailViewModel>();

            var validateUserNames = default(Dictionary<int, ValidateUserNameModel>);
            if (model.IsIPLast3Days || model.Is3MatchesLast7Days)
            {
                var arrUserNames = model.UserNameList.Split(',');
                var userNameDistinct = string.Join(",", arrUserNames.Distinct());
                var validatedList = _commonService.ValidateUsernames(userNameDistinct, out bool isException);
                validateUserNames = validatedList.Distinct(x => x.CustID, y => y);
            }

            Parallel.Invoke(() =>
            {
                if (model.IsIPLast3Days)
                {
                    assBySharedIP = GetAssociationDetectSharedIP(validateUserNames, model);
                }
            },
            () =>
            {
                if (model.Is3MatchesLast7Days)
                {
                    assBySharedMatch = GetAssociationDetectSharedMatches(validateUserNames, model);
                }
            },
            () =>
            {
                var dataCust = _associationDetectionService.GetAssociationDetectionDetail(model);
                assDetails = dataCust?.Select(x => new GetAssociationDetectDetailViewModel(x)) ?? Enumerable.Empty<GetAssociationDetectDetailViewModel>();
            });

            var viewModel = assBySharedIP.Concat(assBySharedMatch).Concat(assDetails);

            if (viewModel.Any())
            {
                result = viewModel.GroupBy(x => x.CTSCustID).Select(x =>
                {
                    var agentInfos = x.Where(y => !string.IsNullOrEmpty(y.AgentInfoJson)).Select(y => y.AgentInfoJson);

                    return new GetAssociationDetectDetailViewModel
                    {
                        CTSCustID = x.Key,
                        CustID = x.FirstOrDefault()?.CustID,
                        UserName = x.FirstOrDefault()?.UserName,
                        TransitiveUsername = x.FirstOrDefault()?.TransitiveUsername,
                        AgentInfoJson = string.Join(",", agentInfos),
                        AssociatedTypes = string.Join(", ",
                                    x.Where(y => !string.IsNullOrEmpty(y.AssociatedTypes))
                                    .Select(y => y.AssociatedTypes))
                    };
                });
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<GetAssociationDetectDetailViewModel> GetAssociationDetectSharedIP(Dictionary<int, ValidateUserNameModel> validateUsernames, GetAssociationDetectionDetailViewModel model)
        {
            if (!string.IsNullOrEmpty(model.UserNameList))
            {
                var assBySharedIP = _associationDetectionService.DetectAssociationByIP(validateUsernames, model.CustIDList);

                if (assBySharedIP.IsNotNullOrEmpty())
                {
                    return assBySharedIP.Where(x => x.FromCTSCustID == model.CTSCustID || x.ToCTSCustID == model.CTSCustID)
                               .Select(x =>
                               {
                                   return new GetAssociationDetectDetailViewModel
                                   {
                                       CustID = x.FromCTSCustID == model.CTSCustID ? x.ToCustID : x.FromCustID,
                                       CTSCustID = x.FromCTSCustID == model.CTSCustID ? x.ToCTSCustID : x.FromCTSCustID,
                                       UserName = x.FromCTSCustID == model.CTSCustID ? x.ToUserName : x.FromUserName,
                                       AssociatedTypes = $"<span class='association-detect-detail-ip' data-fromusername={model.UserName} data-fromcustid={model.CustID} data-ips={x.IP}>IP(last3days)</span>"
                                   };
                               });
                }
            }

            return Enumerable.Empty<GetAssociationDetectDetailViewModel>();
        }

        private IEnumerable<GetAssociationDetectDetailViewModel> GetAssociationDetectSharedMatches(Dictionary<int, ValidateUserNameModel> validateUsernames, GetAssociationDetectionDetailViewModel model)
        {
            if (!string.IsNullOrEmpty(model.UserNameList))
            {
                var custIDList = string.Join(",", validateUsernames.Values.Select(x => x.CustID).Distinct());

                var custSharedMatches = _associationDetectionService.GetAssociationDetectSharedMatches(model.CustID, custIDList);

                if (custSharedMatches.IsNotNullOrEmpty())
                {
                    return custSharedMatches.Distinct().Select(x =>
                    {
                        var custFirst = validateUsernames.GetValueOrDefault((int)x);

                        if (custFirst != null)
                        {
                            return new GetAssociationDetectDetailViewModel
                            {
                                UserName = custFirst.UserName,
                                CustID = custFirst.CustID,
                                CTSCustID = custFirst.CTSCustID,
                                AssociatedTypes = $"<span class='association-detect-detail-match' data-fromusername={model.UserName} data-fromcustid={model.CustID}>Match</span>"
                            };
                        }

                        return new GetAssociationDetectDetailViewModel();
                    });
                }
            }

            return Enumerable.Empty<GetAssociationDetectDetailViewModel>();
        }
        #endregion

        #region Detail IP

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociationDetectionDetailIP)]
        public ActionResult GetDetectAssociationDetailIP(string ips)
        {
            var result = new List<GetAssociationDetectDetailIPViewModel>();

            if (!string.IsNullOrEmpty(ips))
            {
                result = ips.Split(',').OrderBy(ip => ip).Select(ip => new GetAssociationDetectDetailIPViewModel { IP = ip }).ToList();
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Detail Match
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociationDetectionDetailMatch)]
        public ActionResult GetDetectAssociationDetailMatch(int custID1, int custID2, string custIDList)
        {
            var result = Enumerable.Empty<GetAssociationDetectDetailMatchViewModel>();
            var tickets = Enumerable.Empty<GetAssociationDetectDetailMatchModel>();

            if (string.IsNullOrEmpty(custIDList))
            {
                custIDList = $"{custID1},{custID2}";

                tickets = _associationDetectionService.GetDetectAssociationDetailMatch(-1, custIDList, (int)DetectAssociation_QueryType.GetTicketDetail);
            }
            else
            {
                tickets = _associationDetectionService.GetDetectAssociationDetailMatch(custID1, custIDList, (int)DetectAssociation_QueryType.GetTicketDetailByListCust);
            }

            result = tickets.Distinct(x => x.TransID)
                .OrderByDescending(x => x.MatchID)
                .ThenByDescending(x => x.TransDate)?
                .Select(x => { return new GetAssociationDetectDetailMatchViewModel(x); });

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Detail Agent
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociationDetectionDetailAgent)]
        public ActionResult GetDetectAssociationDetailAgent(string agentInfoJson)
        {
            var result = default(List<GetAssociationDetectDetailAgentViewModel>);

            if (!string.IsNullOrEmpty(agentInfoJson))
            {
                result = JsonConvert.DeserializeObject<List<GetAssociationDetectDetailAgentViewModel>>(agentInfoJson);
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Export to Excel
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociationDetectionExportToExcel)]
        public ActionResult ExportToExcelAssociationDetect(DetectAssociationByUserNamesViewModel model)
        {
            var dateTime = DateTime.UtcNow.AddHours(-4);

            var fileName = string.Format("AssociationDetect_{0}_{1}.xlsx"
                    , dateTime.ToString(FormatDateTimeToString_YYYYmmdd)
                    , dateTime.ToString(Consts.FormatTimeToString_hhmmss));

            var excelFile = new byte[0];

            try
            {
                var templatePath = Server.MapPath(AssociationDetectionTemplate);

                var data = PrepareDataExportDetectAssociation(model);

                excelFile = _associationDetectionService.ExportToExcelAssociationDetect(data.ExportItemList, fileName, templatePath);

                return File(excelFile, "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {
                ex.LogError();
            }

            return File(excelFile, "application/vnd.ms-excel", fileName);
        }

        private IEnumerable<ExportAssociationDetectModel> GetExportItem(IEnumerable<ValidateUserNameModel> orgUserList
            , IEnumerable<DetectAssociationMergeGroupModel> assMergeGroup
            , IEnumerable<GetCustStatisticsInfoRealtimeModel> custStatisticsList)
        {
            if (assMergeGroup.Any())
            {
                var assMergeGroupDictionary = assMergeGroup.Distinct(x => x.CustID, y => y);
                var custStatisticsDictionary = custStatisticsList.Distinct(x => x.CustID, y => y);

                return orgUserList.Select(x =>
                {
                    var cust1 = assMergeGroupDictionary.GetValueOrDefault(x.CustID);
                    var cust2 = custStatisticsDictionary.GetValueOrDefault(x.CustID);

                    var item = new ExportAssociationDetectModel { UserName = x.UserName };
                    if (cust1 != null)
                    {
                        item.CreatedDate = cust1.CreatedDate;
                        item.CustomerClass = cust1.CustomerClass;
                        item.CustomerCategory = cust1.CustomerCategory;
                        item.GroupID = cust1.GroupID;
                        item.Danger1 = cust1.Danger1;
                        item.Danger2 = cust1.Danger2;
                        item.Danger3 = cust1.Danger3;
                        item.Danger4 = cust1.Danger4;
                        item.Danger5 = cust1.Danger5;
                        item.BotTransSum = cust1.BotTransSum;
                        item.BotTransPercentage = cust1.BotTransPercentage;
                        item.InvalidBrowserInfoTransSum = cust1.InvalidBrowserInfoTransSum;
                        item.InvalidBrowserInfoTransPercentage = cust1.InvalidBrowserInfoTransPercentage;
                    }
                    if (cust2 != null)
                    {
                        item.TotalSettledTickets = cust2.TotalSettledTickets;
                        item.TotalWinloss = cust2.TotalWinlossRM;
                        item.WinlossLast3Days = cust2.WinlossRM_Last3Days;
                        item.WinlossLast7Days = cust2.WinlossRM_Last7Days;
                        item.TotalSabaTurnoverRM = cust2.TotalSabaTurnoverRM;
                        item.SabaTurnoverRMLast3Days = cust2.SabaTurnoverRM_Last3Days;
                        item.SabaTurnoverRMLast7Days = cust2.SabaTurnoverRM_Last7Days;
                        item.TotalTurnover = cust2.TotalTurnoverRM;
                        item.TurnoverLast3Days = cust2.TurnoverRM_Last3Days;
                        item.TurnoverLast7Days = cust2.TurnoverRM_Last7Days;
                    }

                    return item;
                });
            }


            return new List<ExportAssociationDetectModel>();
        }

        #endregion

    }
}
