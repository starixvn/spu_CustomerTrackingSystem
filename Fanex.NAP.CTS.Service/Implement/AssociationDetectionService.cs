namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Helpers;
    using Fanex.NAP.CTS.Core.Service;
    using Fanex.NAP.CTS.Criteria.AssociationDetection;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.AssociationDetection;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Model.DangerMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.AssociationDetection;
    using Fanex.NAP.TGRS.Core.ExcelUtility;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IAssociationDetectionService
    {
        IEnumerable<ValidateUserNameModel> ValidateUserName(string userNameString);

        IEnumerable<DetectAssociationMergeGroupModel> DetectAssociation(DetectAssociationByUserNamesViewModel model, IEnumerable<ValidateUserNameModel> orgUserList, bool IsGetLogTransInfo);

        IEnumerable<DetectAssociationByPrimaryModel> DetectAssociationByPrimary(DetectAssociationByPrimaryFilterModel filter);

        IEnumerable<DetectAssociationByPrimaryModel> DetectAssociationByAgent(string ctsCustIDs);

        IEnumerable<long> GetAssociationDetectSharedMatches(int rootCustID, string custIDList);

        IEnumerable<GetAssociationDetectDetailModel> GetAssociationDetectionDetail(GetAssociationDetectionDetailViewModel assDetectionDetailModel);

        IEnumerable<SharedIPInfoModel> DetectAssociationByIP(Dictionary<int, ValidateUserNameModel> validateUserNames, string custIds);

        IEnumerable<DetectAssociationBySharedIPMatchesModel> DetectAssociationBySharedMatches(string custIDList);

        IEnumerable<DetectAssociationMergeGroupModel> DetectGroupDetectAssociation(MergeGroupInfoModel model);

        IEnumerable<GetCustStatisticsInfoRealtimeModel> GetCustStatisticsInfoRealtime(string custIDList);

        IEnumerable<GetAssociationDetectDetailMatchModel> GetDetectAssociationDetailMatch(long rootCustID, string custIDList, int queryType);

        byte[] ExportToExcelAssociationDetect(IEnumerable<ExportAssociationDetectModel> AssociationDetect, string fileName, string templatePath);

        string DetectAssociationCustByUserRoot(DangerMonitorSearchModel filter);
    }

    public class AssociationDetectionService : IAssociationDetectionService
    {
        private readonly ICriteriaService _repository;
        private readonly ICommonService _commonService;

        public AssociationDetectionService()
        {
            _repository = new CriteriaService();
            _commonService = new CommonService();
        }

        #region Public Methods

        public IEnumerable<ValidateUserNameModel> ValidateUserName(string userNameString)
        {
            var userNames = userNameString.Split(',');

            var userNameList = string.Join(",", userNames.Select(x => x.ToUpper()).Distinct());

            var orgUserList = userNames.Select(x => new ValidateUserNameModel { UserName = x });

            var validatedList = _commonService.ValidateUsernames(userNameList, out bool isException);

            orgUserList = orgUserList.Join(validatedList, c1 => c1.UserName.ToLower(), c2 => c2.UserName.ToLower(),
                (c1, c2) => new ValidateUserNameModel
                {
                    UserName = c2.UserName,
                    CustID = c2.CustID,
                    CTSCustID = c2.CTSCustID,
                    IsInternal = c2.IsInternal
                });

            return orgUserList;
        }

        public IEnumerable<DetectAssociationMergeGroupModel> DetectAssociation(DetectAssociationByUserNamesViewModel model, IEnumerable<ValidateUserNameModel> orgUserList, bool IsGetLogTransInfo)
        {
            var validateUserNames = model.IsIPLast3Days || model.Is3MatchesLast7Days ?
                orgUserList.Where(x => !x.IsInternal).Distinct(x => x.CustID, y => y) : null;

            var custIDs = string.Join(",", orgUserList.Select(x => x.CustID).Distinct());

            var ctsCustIDs = string.Join(",", orgUserList.Select(x => x.CTSCustID).Distinct());

            var mergeGroupInfo = new MergeGroupInfoModel { CTSCustIDs = ctsCustIDs, IsGetLogTransInfo = IsGetLogTransInfo };

            Parallel.Invoke(() =>
            {
                var assBySharedIP = model.IsIPLast3Days ? DetectAssociationByIP(validateUserNames, custIDs) : Enumerable.Empty<SharedIPInfoModel>();
                mergeGroupInfo.AssociationBySharedIPJson = GetAssociationCustByIP(assBySharedIP);
            }, () =>
            {
                var assBySharedMatches = model.Is3MatchesLast7Days ? DetectAssociationBySharedMatches(validateUserNames, custIDs) : Enumerable.Empty<SharedIPMatchesInfoModel>();
                mergeGroupInfo.AssociationBySharedMatchesJson = GetAssociationCustByMatch(assBySharedMatches);
            }, () =>
            {

                mergeGroupInfo.AssociationByDeviceJson = model.IsDevice ? GetAssociationCustByPrimary(new DetectAssociationByPrimaryFilterModel(ctsCustIDs, AssociationDetectionType.Device)) : "[]";
            }, () =>
            {

                mergeGroupInfo.AssociationByAIJson = model.IsBP ? GetAssociationCustByPrimary(new DetectAssociationByPrimaryFilterModel(ctsCustIDs, AssociationDetectionType.AI)) : "[]";
            }, () =>
            {

                mergeGroupInfo.AssociationByIPJson = model.IsIP ? GetAssociationCustByPrimary(new DetectAssociationByPrimaryFilterModel(ctsCustIDs, AssociationDetectionType.IP)) : "[]";
            }, () =>
            {

                mergeGroupInfo.AssociationByAgentJson = model.IsAgent ? GetAssociationCustByAgent(ctsCustIDs) : "[]";
            });

            var assMergeGroup = DetectGroupDetectAssociation(mergeGroupInfo);

            return assMergeGroup;
        }

        public IEnumerable<DetectAssociationByPrimaryModel> DetectAssociationByPrimary(DetectAssociationByPrimaryFilterModel filter)
        {
            var criteria = new DetectAssociationByPrimaryCriteria
            {
                CTSCustIDs = filter.CTSCustIDs,
                IsAI = filter.IsAI,
                IsDevice = filter.IsDevice,
                IsIP = filter.IsIP
            };

            return _repository.Fetch<DetectAssociationByPrimaryModel>(criteria);
        }

        public IEnumerable<DetectAssociationByPrimaryModel> DetectAssociationByAgent(string ctsCustIDs)
        {
            var criteria = new DetectAssociationByAgentCriteria
            {
                CTSCustIDs = ctsCustIDs
            };

            return _repository.Fetch<DetectAssociationByPrimaryModel>(criteria);
        }

        public IEnumerable<DetectAssociationMergeGroupModel> DetectGroupDetectAssociation(MergeGroupInfoModel model)
        {
            var criteria = new DetectAssociationMergeGroupCriteria
            {
                CTSCustIDs = model.CTSCustIDs,
                DeviceJson = model.AssociationByDeviceJson,
                AIJson = model.AssociationByAIJson,
                IPJson = model.AssociationByIPJson,
                AgentJson = model.AssociationByAgentJson,
                SharedIPJson = model.AssociationBySharedIPJson,
                SharedMatchesJson = model.AssociationBySharedMatchesJson,
                IsGetLogTransInfo = model.IsGetLogTransInfo
            };

            return _repository.Fetch<DetectAssociationMergeGroupModel>(criteria);
        }

        public IEnumerable<GetCustStatisticsInfoRealtimeModel> GetCustStatisticsInfoRealtime(string custIDList)
        {
            var criteria = new GetMainCustStatisticsInfoCriteria
            {
                CustIDList = custIDList,
                IsResetBalance = true
            };

            return _repository.Fetch<GetCustStatisticsInfoRealtimeModel>(criteria);
        }

        public IEnumerable<DetectAssociationBySharedIPMatchesModel> DetectAssociationBySharedMatches(string custIDList)
        {
            var criteria = new DetectAssociationBySharedMatchesCriteria
            {
                CustIDList = custIDList,
                QueryType = (int)DetectAssociation_QueryType.DetectAss
            };

            return _repository.Fetch<DetectAssociationBySharedIPMatchesModel>(criteria);
        }

        public IEnumerable<SharedIPInfoModel> DetectAssociationByIP(Dictionary<int, ValidateUserNameModel> validateUsernames, string custIds)
        {
            var detectAssByIP = DetectAssociationByIP(custIds);

            var mergeIP_DetectAssByIP = detectAssByIP.GroupBy(x => new { x.CustID1, x.CustID2 })
                .Select(g => new DetectAssociationBySharedIPModel
                {
                    CustID1 = g.Key.CustID1,
                    CustID2 = g.Key.CustID2,
                    IP = g.Select(x => x.IP.Trim()).Join()
                });

            return DetectAssByIPMappingToSharedIPInfo(mergeIP_DetectAssByIP, validateUsernames);
        }

        private IEnumerable<SharedIPInfoModel> DetectAssByIPMappingToSharedIPInfo(IEnumerable<DetectAssociationBySharedIPModel> detectAssByIP, Dictionary<int, ValidateUserNameModel> validateUsernames)
        {
            return detectAssByIP.Select(x =>
            {
                var fromCTSCust = validateUsernames.GetValueOrDefault(x.CustID1) ?? new ValidateUserNameModel();
                var toCTSCust = validateUsernames.GetValueOrDefault(x.CustID2) ?? new ValidateUserNameModel();

                return new SharedIPInfoModel
                {
                    FromCustID = x.CustID1,
                    FromCTSCustID = fromCTSCust.CTSCustID,
                    FromUserName = fromCTSCust.UserName,
                    ToCustID = x.CustID2,
                    ToCTSCustID = toCTSCust.CTSCustID,
                    ToUserName = toCTSCust.UserName,
                    IP = x.IP
                };
            }).Where(x => x.FromCTSCustID != 0 && x.ToCTSCustID != 0);
        }

        public IEnumerable<long> GetAssociationDetectSharedMatches(int rootCustID, string custIDList)
        {
            var criteria = new DetectAssociationBySharedMatchesCriteria
            {
                RootCustID = rootCustID,
                CustIDList = custIDList,
                QueryType = (int)DetectAssociation_QueryType.GetAssByGroup
            };

            return _repository.Fetch<long>(criteria);
        }

        public IEnumerable<GetAssociationDetectDetailModel> GetAssociationDetectionDetail(GetAssociationDetectionDetailViewModel model)
        {
            var ctsCustIDs = model.CTSCustIDList.Split(',').Select(long.Parse);
            var ctsCustIDList = string.Join(",", ctsCustIDs.Where(x => x != model.CTSCustID));

            var criteria = new GetAssociationDetectionDetailCriteria
            {
                CTSCustID = model.CTSCustID,
                ListCTSCustID = ctsCustIDList,
                IsBP = model.IsBP,
                IsDevice = model.IsDevice,
                IsIP = model.IsIP,
                IsAgent = model.IsAgent
            };

            return _repository.Fetch<GetAssociationDetectDetailModel>(criteria);
        }

        public IEnumerable<GetAssociationDetectDetailMatchModel> GetDetectAssociationDetailMatch(long rootCustID, string custIDList, int queryType)
        {
            var criteria = new DetectAssociationBySharedMatchesCriteria
            {
                CustIDList = custIDList,
                QueryType = queryType
            };

            if (rootCustID != -1)
            {
                criteria.RootCustID = rootCustID;
            }

            return _repository.Fetch<GetAssociationDetectDetailMatchModel>(criteria);
        }

        public byte[] ExportToExcelAssociationDetect(IEnumerable<ExportAssociationDetectModel> associationDetect, string fileName, string templatePath)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, "AD");
            writer.CopyRows(0, 1);

            if (associationDetect.IsNotNullOrEmpty())
            {
                FillRowData(associationDetect, ref writer);
            }

            writer.SyncColumnsWidth(17);

            return writer.ToBytes();
        }
       
        public string DetectAssociationCustByUserRoot(DangerMonitorSearchModel filter)
        {            
            var assFilter = new DetectAssociationByUserRootFilterModel
            {
                UserNames = filter.UserNameList,
                //IsAI = filter.IsAI,   Tobe continue...
                //IsDevice = filter.IsDevice, Tobe continue...
                //IsIP = filter.IsIP, Tobe continue...
                IsAI = true,
                IsDevice = true,
                IsIP = true
            };

            var assWithRootCust = GetAssociationCustByUserRoot(assFilter);

            return JsonConvert.SerializeObject(assWithRootCust);
        }
        #endregion

        #region Private Methods
        private IEnumerable<DetectAssociationByUserRootModel> GetAssociationCustByUserRoot(DetectAssociationByUserRootFilterModel assFilter)
        {
            var criteria = new DetectAssociationByUserRootCriteria
            {
                UserNames = assFilter.UserNames,
                IsAI = assFilter.IsAI,
                IsDevice = assFilter.IsDevice,
                IsIP = assFilter.IsIP
            };

            return _repository.Fetch<DetectAssociationByUserRootModel>(criteria);
        }

        private void FillRowData(IEnumerable<ExportAssociationDetectModel> AssociationDetect, ref ExcelTemplateWriter writer)
        {
            var idx = 1;

            foreach (var item in AssociationDetect)
            {
                var record = new object[]
                    {
                        idx++,
                        item.UserName,
                        item.CreatedDate.ToString(Consts.FormatDateToString_MMMddyyyy),
                        item.CustomerCategory = item.CustomerCategory,
                        item.CustomerClass != -1 ? item.CustomerClass.ToString() : string.Empty,
                        item.DangerLevel,
                        (item.GroupID.HasValue ? item.GroupID.ToString() : string.Empty),
                        item.TotalSettledTickets ?? 0,
                        item.BotTransSum,
                        item.BotTransPercentage.ToString() + " %",
                        item.InvalidBrowserInfoTransSum,
                        item.InvalidBrowserInfoTransPercentage.ToString() + " %",
                        item.TotalWinloss,
                        item.TotalSabaTurnoverRM,
                        item.TotalTurnover
                    };

                writer.WriteRow(1, record);
            }
        }

        private IEnumerable<DetectAssociationBySharedIPModel> DetectAssociationByIP(string custIDList)
        {
            var criteria = new DetectAssociationByIPCriteria
            {
                ListCustId = custIDList
            };

            return _repository.Fetch<DetectAssociationBySharedIPModel>(criteria);
        }

        private string GetAssociationCustByIP(IEnumerable<SharedIPInfoModel> ass)
        {
            var assCustBySharedIP = ass.Select(x => new AssociationCustByIPViewModel
            {               
                O = x.FromCTSCustID,               
                D = x.ToCTSCustID              
            });

            return ObjectHelper.SerializeObj(assCustBySharedIP);
        }

        private IEnumerable<SharedIPMatchesInfoModel> DetectAssociationBySharedMatches(Dictionary<int, ValidateUserNameModel> validateUserNames, string custIds)
        {
            var detectAssByAssMatches = DetectAssociationBySharedMatches(custIds);

            return detectAssByAssMatches.Select(x =>
            {
                var fromCTSCust = validateUserNames.GetValueOrDefault(x.CustID1) ?? new ValidateUserNameModel();
                var toCTSCust = validateUserNames.GetValueOrDefault(x.CustID2) ?? new ValidateUserNameModel();

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

        private string GetAssociationCustByMatch(IEnumerable<SharedIPMatchesInfoModel> ass)
        {
            var assCustBySharedIP = ass.Select(x => new AssociationCustModel
            {               
                FromCTSCustID = x.FromCTSCustID,              
                ToCTSCustID = x.ToCTSCustID                
            });

            return JsonConvert.SerializeObject(assCustBySharedIP);
        }

        private string GetAssociationCustByPrimary(DetectAssociationByPrimaryFilterModel custAssType)
        {
            var assByPrimary = DetectAssociationByPrimary(custAssType);

            var input = assByPrimary.Select(c => new
            {
                O = c.OrigID,
                D = c.DestID,
                A = c.AssociationType
            });

            return ObjectHelper.SerializeObj(input);
        }

        private string GetAssociationCustByAgent(string ctsCustIDs)
        {
            var assByAgent = DetectAssociationByAgent(ctsCustIDs);

            var input = assByAgent.Select(c => new
            {
                O = c.OrigID,
                D = c.DestID,
                A = c.AssociationType
            });

            return ObjectHelper.SerializeObj(input);
        }

        #endregion
    }
}
