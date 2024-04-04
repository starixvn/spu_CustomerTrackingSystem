namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Helpers;    
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.AssociationDetection;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Resource.Consts;
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
            var result = DetectAssociation(model, false);

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

        private DetectAssociationViewModel DetectAssociation(DetectAssociationByUserNamesViewModel model, bool isExportExcel)
        {
            var result = new DetectAssociationViewModel();

            if (!string.IsNullOrWhiteSpace(model.UserNames))
            {
                try
                {
                    var orgUserList = _associationDetectionService.ValidateUserName(model.UserNames);

                    var assMergeGroup = _associationDetectionService.DetectAssociation(model, orgUserList, isExportExcel, true);

                    var assRearrangeGroup = RearrangeAssGroup(assMergeGroup, isExportExcel);

                    if (isExportExcel)
                    {
                        var custIDs = string.Join(",", orgUserList.Select(x => x.CustID).Distinct());

                        var custStatisticsList = _associationDetectionService.GetCustStatisticsInfoRealtime(custIDs);

                        result.ExportItemList = GetExportItem(orgUserList, assRearrangeGroup, custStatisticsList);
                    }
                    else
                    {
                        result.ItemList = assRearrangeGroup.Select(x =>
                        {
                            return new DetectAssociationItemViewModel(x);
                        });
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorCode = 1;
                    result.ErrorMessage = ex.Message;

                    var logMessage = new LogMessage(ex.Message)
                       .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error(logMessage);
                }
            }

            return result;
        }

        private IEnumerable<DetectAssociationMergeGroupModel> RearrangeAssGroup(IEnumerable<DetectAssociationMergeGroupModel> assGroupList, bool isExportExcel)
        {
            var result = new List<DetectAssociationMergeGroupModel>();
            if (!isExportExcel)
                assGroupList = assGroupList.Where(x => x.GroupID.HasValue);

            if (assGroupList.Any())
            {
                var grouping = assGroupList
                    .GroupBy(x => x.GroupID, (key, g) => new { GroupID = key, Items = g.ToList() })
                    .OrderBy(x => x.GroupID);

                var idx = 1;

                foreach (var group in grouping)
                {
                    var groupID = group.GroupID;

                    if (groupID.HasValue)
                    {
                        groupID = idx;
                        idx += 1;
                    }

                    result.AddRange(group.Items.Select(x =>
                    {
                        x.GroupID = groupID;

                        return x;
                    }));
                }
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
            var viewModel = new List<GetAssociationDetectDetailViewModel>();
            
            if (model.IsIPLast3Days)
            {
                viewModel.AddRange(GetAssociationDetectSharedIP(model.UserNameList, model.CustIDList, model.UserName, model.CustID, model.CTSCustID));
            }

            if (model.Is3MatchesLast7Days)
            {
                viewModel.AddRange(GetAssociationDetectSharedMatches(model));
            }
          
            var dataCust = _associationDetectionService.GetAssociationDetectionDetail(model);
            
            if (dataCust != null && dataCust.Any())
            {
                foreach (var item in dataCust)
                {
                    viewModel.Add(new GetAssociationDetectDetailViewModel(item));
                }     
            }            

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
                                    x.Where(y=>!string.IsNullOrEmpty(y.AssociatedTypes))
                                    .Select(y => y.AssociatedTypes))
                    };
                });
            }
            
            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<GetAssociationDetectDetailViewModel> GetAssociationDetectSharedIP(string userNames, string custIDs, string userName, int custID, long ctsCustID)
        {
            var validatedList = _commonService.ValidateUsernames(userNames, out bool isException);

            var assBySharedIP = _associationDetectionService.DetectAssociationByIP(validatedList, custIDs);
          
            var result = assBySharedIP.Where(x => x.FromCTSCustID == ctsCustID || x.ToCTSCustID == ctsCustID)
                       .Select(x =>
                       {
                           return new GetAssociationDetectDetailViewModel
                           {
                               CustID = x.FromCTSCustID == ctsCustID ? x.ToCustID : x.FromCustID,
                               CTSCustID = x.FromCTSCustID == ctsCustID ? x.ToCTSCustID : x.FromCTSCustID,
                               UserName = x.FromCTSCustID == ctsCustID ? x.ToUserName : x.FromUserName,
                               AssociatedTypes = $"<span class='association-detect-detail-ip' data-fromusername={userName} data-fromcustid={custID} data-ips={x.IP}>IP(last3days)</span>"
                           };
                       });

            return result;
        }
      
        private IEnumerable<GetAssociationDetectDetailViewModel> GetAssociationDetectSharedMatches(GetAssociationDetectionDetailViewModel model)
        {
            if (!string.IsNullOrEmpty(model.UserNameList))
            {
                var isException = false;

                var arrUserNames = model.UserNameList.Split(',');

                var userNameDistinct = string.Join(",", arrUserNames.Distinct());

                var validatedList = _commonService.ValidateUsernames(userNameDistinct, out isException);

                var custIDList = string.Join(",", validatedList.Select(x => x.CustID).Distinct());

                var custSharedMatches = _associationDetectionService.GetAssociationDetectSharedMatches(model.CustID, custIDList);

                if (custSharedMatches != null)
                {
                    return custSharedMatches.Distinct().Select(x =>
                    {
                        var custFirst = validatedList.FirstOrDefault(y => y.CustID == x);

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
                var arrIP = ips.Split(',');

                foreach (var ip in arrIP)
                {
                    result.Add(new GetAssociationDetectDetailIPViewModel { IP = ip });
                }

                result = result.OrderBy(x => x.IP).ToList();
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
            
            tickets = tickets.Distinct(x => x.TransID)
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

        #endregion

        #region Detail Agent
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociationDetectionDetailAgent)]
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

        #endregion

        #region Export to Excel
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociationDetectionExportToExcel)]
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

        private IEnumerable<ExportAssociationDetectModel> GetExportItem(IEnumerable<ValidateUserNameModel> orgUserList
            , IEnumerable<DetectAssociationMergeGroupModel> assMergeGroup
            , IEnumerable<GetCustStatisticsInfoRealtimeModel> custStatisticsList)
        {            
            if (assMergeGroup.Any())
            {
                return orgUserList.Select(x =>
                {
                    var cust1 = assMergeGroup.FirstOrDefault(y => y.CustID == x.CustID);
                    var cust2 = custStatisticsList.FirstOrDefault(y => y.CustID == x.CustID);
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
