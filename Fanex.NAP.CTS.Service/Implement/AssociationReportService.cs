namespace Fanex.NAP.CTS.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Criteria.Account;
    using Fanex.NAP.CTS.Criteria.Account.Associated;
    using Fanex.NAP.CTS.Criteria.AssociationReport;
    using Fanex.NAP.CTS.Criteria.Common;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.AssociationReport;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.ViewModel.AssociationReport;

    public class AssociationReportService : IAssociationReportService
    {
        public List<CustomerStaticStatusModel> GetCustomerStaticStatus(int listId, bool status)
        {
            var criteria = new CustomerStaticAccountStatusCriteria
            {
                ListId = listId,
                Status = status
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.Query<CustomerStaticStatusModel>(criteria);

                return result.ToList();
            }
        }

        public IEnumerable<AccountStaticStatusModel> GetAccountStaticStatuses()
        {
            var statusList = new List<AccountStaticStatusModel>();

            statusList.Add(new AccountStaticStatusModel
            {
                Ids = "1,11", // 1:Open, 11:Active
                Name = "Open"
            });

            statusList.Add(new AccountStaticStatusModel
            {
                Ids = "2,3,4,12,13,14", // 2:Disabled, 3:Closed, 4:Suspended, 12:Inactive, 13:View Only, 14:Suspended
                Name = "Closed"
            });

            return statusList;
        }

        public IEnumerable<EvidenceModel> GetEvidenceList()
        {
            var criteria = new GetAllEvidenceCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<EvidenceModel>(criteria);
            }
        }

        public GraphAssociationModel GetAssociations(string apiEndPoint, AssociationFilterViewModel filter)
        {
            try
            {
                var param = new
                {
                    AssociatedDateFrom = filter.FromDate,
                    AssociatedDateTo = filter.ToDate,
                    AccountStatuses = filter.AccountStatusIds,
                    filter.SiteName,
                    filter.AssociationStatus,
                    filter.EvidenceCode,
                    filter.AssociationType,
                    TakeRow = filter.length,
                    SkipRow = filter.start
                };

                var data = ApiHelper.Post<GraphAssociationModel> (apiEndPoint, param);

                Task.WhenAll(data);

                return data.Result ?? new GraphAssociationModel();
            }
            catch (Exception ex)
            {
                LogService.Error("CTS - AssociationReportService GetAssociations - Exception: " + ex.Message, ex);
            }

            return new GraphAssociationModel();
        }

        public IEnumerable<CategoriesModel> GetCategories(string ctsCustIdList)
        {
            var criteria = new AssociationReportGetCategoriesCriteria
            {
                CTSCustIdList = ctsCustIdList
            };
            try
            {
                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    return db.Query<CategoriesModel>(criteria);
                }
            }
            catch (Exception ex)
            {
                LogService.Error("CTS - AssociationReportService GetCategories - Exception: " + ex.Message, ex);
            }

            return new List<CategoriesModel>();
        }

        public GraphAssociationDetailModel GetAssociationDetail(string apiEndPoint, AssociationDetailFilterViewModel filter)
        {
            try
            {
                var param = new
                {
                    filter.CTSCustId,
                    AssociatedDateFrom = filter.FromDate,
                    AssociatedDateTo = filter.ToDate,
                    filter.AssociationStatus,
                    filter.AssociationType,
                    TakeRow = filter.length,
                    SkipRow = filter.start
                };

                var data = ApiHelper.Post<GraphAssociationDetailModel>(apiEndPoint, param);

                Task.WhenAll(data);

                return data.Result ?? new GraphAssociationDetailModel();
            }
            catch (Exception ex)
            {
                LogService.Error("CTS - AssociationReportService GetAssociationDetail - Exception: " + ex.Message, ex);
            }

            return new GraphAssociationDetailModel();
        }       

        public DBResult ActiveRemoveAssociation(int fromSubscriberId, long fromCTSCustId, int toSubscriberId, long toCTSCustId, int createdBy, bool isRemoveAction)
        {
            var criteria = new ActiveRemoveAssociationCriteria
            {
                FromSubscriberId = fromSubscriberId,
                FromCTSCustId = fromCTSCustId,
                ToSubscriberId = toSubscriberId,
                ToCTSCustId = toCTSCustId,
                CreatedBy = createdBy,
                IsActionRemove = isRemoveAction
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);

                return new DBResult
                {
                    ErrorCode = string.IsNullOrWhiteSpace(criteria.ErrorMessage) ? 0 : -1,
                    ErrorMsg = criteria.ErrorMessage
                };
            }
        }

        public IEnumerable<AssociationFromMySqlModel> GetAssociationFromMySql(AssociationFilterViewModel filter, out int totalItems)
        {
            totalItems = 0;

            var criteria = new AssociationReportGetCriteria
            {
                FromAssDate = filter.FromDate,
                ToAssDate = filter.ToDate,
                AccountStatusIDs = filter.AccountStatusIds,
                SubscriberIDs = filter.SubscriberIDs,
                AssociationStatus = filter.AssociationStatus.ToString(),
                EvidenceIDs = filter.EvidenceIDs,
                AssociationType = filter.AssociationType,
                TakeRow = filter.length,
                SkipRow = filter.start
            };

            var repository = new DynamicRepository();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var data = repository.Fetch<AssociationFromMySqlModel>(criteria);

                totalItems = criteria.TotalItems;

                return data;
            }
        }

        public IEnumerable<AssociationDetailFromMySqlModel> GetAssociationDetailFromMySql(AssociationDetailFilterViewModel filter, out int totalItems)
        {
            totalItems = 0;

            var criteria = new AssociationDetailReportGetCriteria
            {
                CTSCustID = filter.CTSCustId,
                FromAssDate = filter.FromDate,
                ToAssDate = filter.ToDate,
                AssociationType = filter.AssociationType,
                TakeRow = filter.length,
                SkipRow = filter.start
            };

            var repository = new DynamicRepository();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var data = repository.Fetch<AssociationDetailFromMySqlModel>(criteria);

                totalItems = criteria.TotalItems;

                return data;
            }
        }

    }
}
