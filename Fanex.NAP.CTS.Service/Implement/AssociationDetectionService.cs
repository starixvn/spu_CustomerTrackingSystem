namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.AssociationDetection;
    using Fanex.NAP.CTS.Model.AssociationDetection;
    using Fanex.NAP.CTS.ViewModel.AssociationDetection;
    using Fanex.NAP.SPU.Graph.Core;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IAssociationDetectionService
    {
        IEnumerable<GetAssociationDetectionModel> GetAssociationDetection(string ctsCustIds, bool fromMySQL = true);

        List<GetAssociationDetectionModel> ClassifyGroupAssociation(IEnumerable<AssociationLevelGraphDbViewModel> association, IEnumerable<long> ip_CTSCustIDs);

        IEnumerable<GetAssociationDetectionModel> GetCustInfoAssDetectionReport(List<GetAssociationDetectionModel> assDetection);

        IEnumerable<DetectAssociationByBPDeviceIPAgentModel> DetectAssociationByBPDeviceIPAgent(DetectAssociationByUserNamesViewModel model);

        IEnumerable<GetAssociationDetectionDetailModel> GetAssociationDetectionDetail(GetAssociationDetectionDetailViewModel assDetectionDetailModel);

        IEnumerable<DetectAssociationByIPModel> DetectAssociationByIP(string custIDList);

        IEnumerable<DetectAssociationBySharedMatchesModel> DetectAssociationBySharedMatches(string custIDList);

        IEnumerable<string> GetCoupleSharedIP(long custID1, long custID2);
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

        public IEnumerable<DetectAssociationByBPDeviceIPAgentModel> DetectAssociationByBPDeviceIPAgent(DetectAssociationByUserNamesViewModel model)
        {
            var criteria = new DetectAssociationByBPDeviceIPAgentCriteria
            {
                Usernames = model.UserNames,
                HasBettingPattern = model.IsBPManuallyDeviceIP,
                HasManually = model.IsBPManuallyDeviceIP,
                HasDevice = model.IsBPManuallyDeviceIP,
                HasIP = model.IsBPManuallyDeviceIP,
                HasAgent = model.IsAgent
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DetectAssociationByBPDeviceIPAgentModel>(criteria);
            }
        }
      
        public IEnumerable<DetectAssociationBySharedMatchesModel> DetectAssociationBySharedMatches(string custIDList)
        {
            var criteria = new DetectAssociationBySharedMatchesCriteria
            {
                CustIDs = custIDList
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DetectAssociationBySharedMatchesModel>(criteria);
            }
        }

        public IEnumerable<DetectAssociationByIPModel> DetectAssociationByIP(string custIDList)
        {
            var criteria = new DetectAssociationByIPCriteria
            {
                CustIDs = custIDList
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DetectAssociationByIPModel>(criteria);
            }
        }

        public IEnumerable<GetAssociationDetectionDetailModel> GetAssociationDetectionDetail(GetAssociationDetectionDetailViewModel assDetectionDetailModel)
        {
            var criteria = new GetAssociationDetectionDetailCriteria
            {
                CTSCustID = assDetectionDetailModel.CTSCustID,
                ListCTSCustID = assDetectionDetailModel.ListCTSCustID,
                HasManually = assDetectionDetailModel.IsBPManuallyDeviceIP,
                HasBettingPattern = assDetectionDetailModel.IsBPManuallyDeviceIP,
                HasDevice = assDetectionDetailModel.IsBPManuallyDeviceIP,
                HasIP = assDetectionDetailModel.IsBPManuallyDeviceIP,
                HasAgent = assDetectionDetailModel.IsAgent
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<GetAssociationDetectionDetailModel>(criteria);
            }
        }

        public IEnumerable<string> GetCoupleSharedIP(long custID1, long custID2)
        {
            var criteria = new GetCoupleSharedIPCriteria
            {
                CustID1 = custID1,
                CustID2 = custID2
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<string>(criteria);
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

        #endregion

    }
}
