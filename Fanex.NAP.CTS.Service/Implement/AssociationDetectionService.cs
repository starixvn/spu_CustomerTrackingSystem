namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.AssociationDetection;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.AssociationDetection;
    using Fanex.NAP.CTS.Model.GraphDB.AssDetection;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.ViewModel.AssociationDetection;
    using Fanex.NAP.SPU.Graph.Core;
    using Fanex.NAP.TGRS.Core.ExcelUtility;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IAssociationDetectionService
    {
        IEnumerable<GetAssociationDetectionModel> GetAssociationDetection(string ctsCustIds, bool fromMySQL = true);

        List<GetAssociationDetectionModel> ClassifyGroupAssociation(IEnumerable<AssociationLevelGraphDbViewModel> association, IEnumerable<long> ip_CTSCustIDs);

        IEnumerable<GetAssociationDetectionModel> GetCustInfoAssDetectionReport(List<GetAssociationDetectionModel> assDetection);

        IEnumerable<DetectAssociationByPrimaryModel> DetectAssociationByPrimary(DetectAssociationByPrimaryFilterModel filter, bool fromMySQL = true);

        IEnumerable<DetectAssociationByPrimaryModel> DetectAssociationByAgent(string ctsCustIDs, bool fromMySQL = true);

        IEnumerable<long> GetAssociationDetectSharedMatches(int rootCustID, string custIDList);

        IEnumerable<GetAssociationDetectDetailModel> GetAssociationDetectionDetail(GetAssociationDetectionDetailViewModel assDetectionDetailModel, bool fromMySQL = true);

        IEnumerable<DetectAssociationBySharedIPMatchesModel> DetectAssociationByIP(string custIDList);

        IEnumerable<DetectAssociationBySharedIPMatchesModel> DetectAssociationBySharedMatches(string custIDList);

        IEnumerable<DetectAssociationMergeGroupModel> DetectGroupDetectAssociation(MergeGroupInfoModel model);

        IEnumerable<GetCustStatisticsInfoRealtimeModel> GetCustStatisticsInfoRealtime(string custIDList);

        IEnumerable<GetAssociationDetectDetailIPModel> GetCoupleSharedIP(int custID1, int custID2);

        IEnumerable<GetAssociationDetectDetailMatchModel> GetDetectAssociationDetailMatch(long rootCustID, string custIDList, int queryType);

        byte[] ExportToExcelAssociationDetect(IEnumerable<ExportAssociationDetectModel> AssociationDetect, string fileName, string templatePath);
    }

    public class AssociationDetectionService : IAssociationDetectionService
    {
        #region Query From MySQL

        public IEnumerable<GetAssociationDetectionModel> GetAssociationDetection(string ctsCustIds, bool fromMySQL = true)
        {
            var result = fromMySQL ? GetAssociationDetectionFromMySQL(ctsCustIds) : GetAssociationDetectionFromGraphDB(ctsCustIds);

            return result ?? new List<GetAssociationDetectionModel>();
        }

        public IEnumerable<GetAssociationDetectionModel> GetAssociationDetectionFromMySQL(string ctsCustIds)
        {
            var criteria = new GetAssociationDetectionCriteria
            {
                CTSCustIds = ctsCustIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<GetAssociationDetectionModel>(criteria);
            }
        }

        public IEnumerable<DetectAssociationByPrimaryModel> DetectAssociationByPrimary(DetectAssociationByPrimaryFilterModel filter, bool fromMySQL = true)
        {
            if (fromMySQL)
            {
                var criteria = new DetectAssociationByPrimaryCriteria
                {
                    CTSCustIDs = filter.CTSCustIDs,
                    IsBP = filter.IsBP,
                    IsDevice = filter.IsDevice,
                    IsIP = filter.IsIP
                };

                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    return db.Query<DetectAssociationByPrimaryModel>(criteria);
                }
            }

            return DetectAssociationByPrimaryFromGraphDB(filter.CTSCustIDs, 1);
        }

        public IEnumerable<DetectAssociationByPrimaryModel> DetectAssociationByAgent(string ctsCustIDs, bool fromMySQL = true)
        {
            if (fromMySQL)
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

            return DetectAssociationByPrimaryFromGraphDB(ctsCustIDs, 2);
        }

        public IEnumerable<DetectAssociationMergeGroupModel> DetectGroupDetectAssociation(MergeGroupInfoModel model)
        {
            var criteria = new DetectAssociationMergeGroupCriteria
            {
                CTSCustIDs = model.CTSCustIDs,
                PrimaryJson = model.AssociationByPrimaryJson,
                AgentJson = model.AssociationByAgentJson,
                SharedIPJson = model.AssociationByIPJson,
                SharedMatchesJson = model.AssociationByMatchesJson
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DetectAssociationMergeGroupModel>(criteria);
            }
        }

        public IEnumerable<GetCustStatisticsInfoRealtimeModel> GetCustStatisticsInfoRealtime(string custIDList)
        {
            var criteria = new GetCustStatisticsInfoRealtimeCriteria
            {
                CustIDList = custIDList
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

        public IEnumerable<DetectAssociationBySharedIPMatchesModel> DetectAssociationByIP(string custIDList)
        {
            var criteria = new DetectAssociationByIPCriteria
            {
                CustIDs = custIDList
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DetectAssociationBySharedIPMatchesModel>(criteria);
            }
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

        public IEnumerable<GetAssociationDetectDetailIPModel> GetCoupleSharedIP(int custID1, int custID2)
        {
            var criteria = new GetCoupleSharedIPCriteria
            {
                CustID1 = custID1,
                CustID2 = custID2
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<GetAssociationDetectDetailIPModel>(criteria);
            }
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

            writer.SyncColumnsWidth(15);

            return writer.ToBytes();
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
                        item.CustomerClass != -1 ? item.CustomerClass.ToString() : string.Empty,
                        (item.GroupID.HasValue ? item.GroupID.ToString() : string.Empty),
                        item.DangerLevel,
                        item.TotalWinloss,
                        item.WinlossLast3Days,
                        item.WinlossLast7Days,
                        item.TotalSabaTurnoverRM,
                        item.SabaTurnoverRMLast3Days,
                        item.SabaTurnoverRMLast7Days,
                        item.TotalTurnover,
                        item.TurnoverLast3Days,
                        item.TurnoverLast7Days
                    };

                writer.WriteRow(1, record);
            }
        }

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
