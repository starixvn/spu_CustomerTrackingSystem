namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core.Helpers;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.AssociationDetection;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.AssociationDetection;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Model.GraphDB.AssDetection;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.AssociationDetection;
    using Fanex.NAP.SPU.Graph.Core;
    using Fanex.NAP.TGRS.Core.ExcelUtility;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IAssociationDetectionService
    {
        IEnumerable<ValidateUserNameModel> ValidateUserName(string userNameString);

        IEnumerable<DetectAssociationMergeGroupModel> DetectAssociation(DetectAssociationByUserNamesViewModel model, IEnumerable<ValidateUserNameModel> orgUserList, bool isExportExcel, bool IsGetLogTransInfo);

        List<GetAssociationDetectionModel> ClassifyGroupAssociation(IEnumerable<AssociationLevelGraphDbViewModel> association, IEnumerable<long> ip_CTSCustIDs);

        IEnumerable<GetAssociationDetectionModel> GetCustInfoAssDetectionReport(List<GetAssociationDetectionModel> assDetection);

        IEnumerable<DetectAssociationByPrimaryModel> DetectAssociationByPrimary(DetectAssociationByPrimaryFilterModel filter);

        IEnumerable<DetectAssociationByPrimaryModel> DetectAssociationByAgent(string ctsCustIDs);

        IEnumerable<long> GetAssociationDetectSharedMatches(int rootCustID, string custIDList);

        IEnumerable<GetAssociationDetectDetailModel> GetAssociationDetectionDetail(GetAssociationDetectionDetailViewModel assDetectionDetailModel, bool fromMySQL = true);

        IEnumerable<SharedIPInfoModel> DetectAssociationByIP(IEnumerable<ValidateUserNameModel> validateUserNames, string custIds);

        IEnumerable<DetectAssociationBySharedIPMatchesModel> DetectAssociationBySharedMatches(string custIDList);

        IEnumerable<DetectAssociationMergeGroupModel> DetectGroupDetectAssociation(MergeGroupInfoModel model);

        IEnumerable<GetCustStatisticsInfoRealtimeModel> GetCustStatisticsInfoRealtime(string custIDList);

        IEnumerable<GetAssociationDetectDetailMatchModel> GetDetectAssociationDetailMatch(long rootCustID, string custIDList, int queryType);

        byte[] ExportToExcelAssociationDetect(IEnumerable<ExportAssociationDetectModel> AssociationDetect, string fileName, string templatePath);
    }

    public class AssociationDetectionService : IAssociationDetectionService
    {
        private readonly ICommonService _commonService;

        public AssociationDetectionService()
        {
            _commonService = new CommonService();
        }

        #region Query From MySQL

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

        public IEnumerable<DetectAssociationMergeGroupModel> DetectAssociation(DetectAssociationByUserNamesViewModel model, IEnumerable<ValidateUserNameModel> orgUserList, bool isExportExcel, bool IsGetLogTransInfo)
        {
            var custIgnoreInternals = orgUserList.Where(x => !x.IsInternal);

            var custIDs = string.Join(",", orgUserList.Select(x => x.CustID).Distinct());

            var ctsCustIDs = string.Join(",", orgUserList.Select(x => x.CTSCustID).Distinct());

            var mergeGroupInfo = new MergeGroupInfoModel { CTSCustIDs = ctsCustIDs, IsGetLogTransInfo = IsGetLogTransInfo };

            Parallel.Invoke(() =>
            {
                var assBySharedIP = model.IsIPLast3Days ? DetectAssociationByIP(custIgnoreInternals, custIDs) : Enumerable.Empty<SharedIPInfoModel>();
                mergeGroupInfo.AssociationBySharedIPJson = GetAssociationCustByIP(assBySharedIP);
            }, () =>
            {
                var assBySharedMatches = model.Is3MatchesLast7Days ? DetectAssociationBySharedMatches(custIgnoreInternals, custIDs) : Enumerable.Empty<SharedIPMatchesInfoModel>();
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

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DetectAssociationByPrimaryModel>(criteria);
            }
        }

        public IEnumerable<DetectAssociationByPrimaryModel> DetectAssociationByAgent(string ctsCustIDs)
        {
            var criteria = new DetectAssociationByAgentCriteria
            {
                CTSCustIDs = ctsCustIDs
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DetectAssociationByPrimaryModel>(criteria);
            }
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

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DetectAssociationMergeGroupModel>(criteria);
            }
        }

        public IEnumerable<GetCustStatisticsInfoRealtimeModel> GetCustStatisticsInfoRealtime(string custIDList)
        {
            var criteria = new GetMainCustStatisticsInfoCriteria
            {
                CustIDList = custIDList,
                IsResetBalance = true
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<GetCustStatisticsInfoRealtimeModel>(criteria);
            }
        }

        public IEnumerable<DetectAssociationBySharedIPMatchesModel> DetectAssociationBySharedMatches(string custIDList)
        {
            var criteria = new DetectAssociationBySharedMatchesCriteria
            {
                CustIDList = custIDList,
                QueryType = (int)DetectAssociation_QueryType.DetectAss
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DetectAssociationBySharedIPMatchesModel>(criteria);
            }
        }

        public IEnumerable<SharedIPInfoModel> DetectAssociationByIP(IEnumerable<ValidateUserNameModel> validateUserNames, string custIds)
        {
            var detectAssByIP = DetectAssociationByIP(custIds);

            var detectAssByIPGroupBy = detectAssByIP.GroupBy(x => new { x.CustID1, x.CustID2 })
                .Select(g => new DetectAssociationBySharedIPModel
                {
                    CustID1 = g.Key.CustID1,
                    CustID2 = g.Key.CustID2,
                    IP = string.Join(",", g.Select(x => x.IP.Trim()))
                });

            return detectAssByIPGroupBy.Select(x =>
            {
                var fromCTSCust = validateUserNames.FirstOrDefault(y => y.CustID.Equals(x.CustID1)) ?? new ValidateUserNameModel();
                var toCTSCust = validateUserNames.FirstOrDefault(y => y.CustID.Equals(x.CustID2)) ?? new ValidateUserNameModel();

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

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<long>(criteria);
            }
        }

        public IEnumerable<GetAssociationDetectDetailModel> GetAssociationDetectionDetail(GetAssociationDetectionDetailViewModel model, bool fromMySQL = true)
        {
            var ctsCustIDs = model.CTSCustIDList.Split(',').Select(long.Parse);
            var ctsCustIDList = string.Join(",", ctsCustIDs.Where(x => x != model.CTSCustID));

            if (fromMySQL)
            {
                var criteria = new GetAssociationDetectionDetailCriteria
                {
                    CTSCustID = model.CTSCustID,
                    ListCTSCustID = ctsCustIDList,
                    IsBP = model.IsBP,
                    IsDevice = model.IsDevice,
                    IsIP = model.IsIP,
                    IsAgent = model.IsAgent
                };

                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    return db.Query<GetAssociationDetectDetailModel>(criteria);
                }
            }

            return GetAssociationDetectionDetailFromGraphDB(model.CTSCustID, ctsCustIDList, model.IsDevice, model.IsAgent);
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

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<GetAssociationDetectDetailMatchModel>(criteria);
            }
        }

        public byte[] ExportToExcelAssociationDetect(IEnumerable<ExportAssociationDetectModel> AssociationDetect, string fileName, string templatePath)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, "AD");
            writer.CopyRows(0, 1);
           
            FillRowData(AssociationDetect, ref writer);

            writer.SyncColumnsWidth(17);

            return writer.ToBytes();
        }

        #endregion

        #region Private Methods

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

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DetectAssociationBySharedIPModel>(criteria);
            }
        }

        private string GetAssociationCustByIP(IEnumerable<SharedIPInfoModel> ass)
        {
            var assCustBySharedIP = ass.Select(x => new AssociationCustByIPViewModel
            {
                F1 = x.FromCustID,
                O = x.FromCTSCustID,
                X = x.FromUserName,
                T1 = x.ToCustID,
                D = x.ToCTSCustID,
                Y = x.ToUserName,
                IP = x.IP
            });

            return ObjectHelper.SerializeObj(assCustBySharedIP);
        }

        private IEnumerable<SharedIPMatchesInfoModel> DetectAssociationBySharedMatches(IEnumerable<ValidateUserNameModel> validateUserNames, string custIds)
        {
            var detectAssByAssMatches = DetectAssociationBySharedMatches(custIds);

            return detectAssByAssMatches.Select(x =>
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

        private string GetAssociationCustByMatch(IEnumerable<SharedIPMatchesInfoModel> ass)
        {
            var assCustBySharedIP = ass.Select(x => new AssociationCustModel
            {
                FromCustID = x.FromCustID,
                FromCTSCustID = x.FromCTSCustID,
                FromUserName = x.FromUserName,
                ToCustID = x.ToCustID,
                ToCTSCustID = x.ToCTSCustID,
                ToUserName = x.ToUserName
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

            return ObjectHelper.SerializeObj(assByAgent);
        }

        #endregion

        #endregion

        #region Query From GraphDB

        public IEnumerable<GetAssociationDetectionModel> GetAssociationDetectionFromGraphDB(string ctsCustIds)
        {
            var ip_CTSCustIDs = ctsCustIds;

            var data = ApiHelper.Post<List<GetAssociationDetectionModel>>(ConfigSettings.GetAssociationDetection, ip_CTSCustIDs, 300);

            Task.WhenAll(data);

            return data.Result ?? new List<GetAssociationDetectionModel>();
        }

        public List<GroupClassificationGraphDbViewModel> GroupClassification(IEnumerable<AssociationLevelGraphDbViewModel> association)
        {
            var groupClassification = new List<GroupClassificationGraphDbViewModel>();

            if (association != null && association.Any())
            {
                association.ToList().ForEach(i =>
                {
                    bool isNewGroup = true;

                    foreach (var j in groupClassification)
                    {
                        if (j.CTSCustIDs.Contains(i.CTSCustIdLv0))
                        {
                            j.CTSCustIDs.AddRange(i.Associations.Except(j.CTSCustIDs));
                            isNewGroup = false;
                            break;
                        }

                        if (i.Associations.Intersect(j.CTSCustIDs).Any())
                        {
                            j.CTSCustIDs.Add(i.CTSCustIdLv0);
                            j.CTSCustIDs.AddRange(i.Associations.Except(j.CTSCustIDs));
                            isNewGroup = false;
                            break;
                        }
                    }

                    if (isNewGroup)
                    {
                        groupClassification.Add(new GroupClassificationGraphDbViewModel
                        {
                            GroupID = groupClassification.Count > 0 ? groupClassification.Max(c => c.GroupID) + 1 : 1,
                            CTSCustIDs = new List<long> { i.CTSCustIdLv0 }.Concat(i.Associations).ToList()
                        });
                    }
                });
            }

            return groupClassification;
        }

        public List<GetAssociationDetectionModel> ClassifyGroupAssociation(IEnumerable<AssociationLevelGraphDbViewModel> association, IEnumerable<long> ip_CTSCustIDs)
        {
            var assDetection = new List<GetAssociationDetectionModel>();

            var groupAssociation = GroupClassification(association);

            if (groupAssociation != null && groupAssociation.Any())
            {
                groupAssociation.ToList().ForEach(i =>
                {
                    var rootCTSCustIds = i.CTSCustIDs.Where(ctsCustId => ip_CTSCustIDs.Contains(ctsCustId)).Distinct();

                    foreach (var rootCTSCustId in rootCTSCustIds)
                    {
                        assDetection.Add(new GetAssociationDetectionModel
                        {
                            GroupId = i.GroupID,
                            CTSCustId = rootCTSCustId
                        });
                    }
                });
            }

            return assDetection;
        }

        public IEnumerable<GetAssociationDetectionModel> GetCustInfoAssDetectionReport(List<GetAssociationDetectionModel> assDetection)
        {
            if (assDetection != null && assDetection.Any())
            {
                var custInfo = GetCustInformation(assDetection);

                if (custInfo.Any())
                {
                    var result = custInfo.Join(
                                    assDetection,
                                    x => x.CTSCustId,
                                    y => y.CTSCustId,
                                    (x, y) => new GetAssociationDetectionModel
                                    {
                                        CTSCustId = x.CTSCustId,
                                        GroupId = y.GroupId,
                                        CategoryIds = x.CategoryIds,
                                        CustId = x.CustId,
                                        CustomerCategory = string.IsNullOrEmpty(x.CustomerCategory) ? "-" : x.CustomerCategory,
                                        CustStatus = x.CustStatus,
                                        CustSubId = x.CustSubId,
                                        Evidence = string.IsNullOrEmpty(x.Evidence) ? "-" : x.Evidence,
                                        ParentIds = x.ParentIds,
                                        UserName = x.UserName
                                    });

                    return result ?? new List<GetAssociationDetectionModel>();
                }
            }

            return new List<GetAssociationDetectionModel>();
        }

        public IEnumerable<GetAssociationDetectionModel> GetCustInformation(List<GetAssociationDetectionModel> assDetection)
        {
            if (assDetection != null && assDetection.Any())
            {
                var ip_CTSCustIDs = assDetection.Select(c => c.CTSCustId);

                var result = DbSessionFactory.Execute<GetAssociationDetectionModel>("GetCustInformation", new { ip_CTSCustIDs = ip_CTSCustIDs }).Data;

                return result ?? new List<GetAssociationDetectionModel>();
            }

            return new List<GetAssociationDetectionModel>();
        }

        public IEnumerable<DetectAssociationByPrimaryModel> DetectAssociationByPrimaryFromGraphDB(string ctsCustIDs, int assType)
        {
            var paras = new DetectAssByPrimaryOrAgentParams
            {
                CTSCustIDs = ctsCustIDs,
                AssType = assType
            };

            var data = ApiHelper.Post<List<DetectAssociationByPrimaryModel>>(ConfigSettings.DetectAssociationByPrimaryOrAgentType, paras);

            Task.WhenAll(data);

            return data.Result ?? new List<DetectAssociationByPrimaryModel>();
        }

        public IEnumerable<GetAssociationDetectDetailModel> GetAssociationDetectionDetailFromGraphDB(long rootCTSCustID, string assCTSCustIDs, bool getPrimaryType, bool getAgentType)
        {
            var paras = new DetectAssGetDetailParams
            {
                RootCTSCustID = rootCTSCustID,
                AssCTSCustIDs = assCTSCustIDs,
                GetPrimaryType = getPrimaryType,
                GetAgentType = getAgentType
            };

            var data = ApiHelper.Post<List<GetAssociationDetectDetailModel>>(ConfigSettings.DetectAssociationGetDetail, paras);

            Task.WhenAll(data);

            return data.Result ?? new List<GetAssociationDetectDetailModel>();           
        }

        #endregion

    }
}
