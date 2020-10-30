namespace Fanex.NAP.CTS.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Criteria.Account;
    using Fanex.NAP.CTS.Criteria.Account.Associated;
    using Fanex.NAP.CTS.Criteria.AssociationReport;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.AssociationReport;
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

        public IEnumerable<AssociationModel> GetAssociations(string apiEndPoint, AssociationFilterViewModel filter)
        {
            //var paramFormat = "AssociatedDateFrom={0}&AssociatedDateTo={1}&AccountStatus={2}&SiteName={3}&AssociationStatus={4}&EvidenceCode={5}&AssociationType={6}";
            //var param = string.Format(paramFormat, filter.FromDate, filter.ToDate, filter.AccountStatusIds, filter.SiteName, filter.AssociationStatuses, filter.EvidenceCode, filter.AssociationType);
            //var urlString = string.Format(apiEndPoint + "?" + param);

            //try
            //{
            //    var data = ApiHelper.Get<IEnumerable<AssociationModel>>(urlString);

            //    Task.WhenAll(data);

            //    return data.Result ?? new List<AssociationModel>();
            //}
            //catch (Exception ex)
            //{
            //    LogService.Error("CTS - AssociationReportService GetAssociations - Exception: " + ex.Message, ex);
            //}

            //return new List<AssociationModel>();

            return GenerateTest();
        }

        private IEnumerable<AssociationModel> GenerateTest()
        {
            var list = new List<AssociationModel>();

            list.Add(new AssociationModel
            {
                CustId = 1,
                CTSCustId = 1,
                AccountStatus = "AccountStatus1",
                CategoryNames = "CategoryNames1",
                EvidenceCode = "EvidenceCode1",
                SiteId = 1,
                SiteName = "SiteName1",
                UserName = "UserName1"
            });

            list.Add(new AssociationModel
            {
                CustId = 2,
                CTSCustId = 2,
                AccountStatus = "AccountStatus2",
                CategoryNames = "CategoryNames2",
                EvidenceCode = "EvidenceCode2",
                SiteId = 2,
                SiteName = "SiteName2",
                UserName = "UserName2"
            });

            list.Add(new AssociationModel
            {
                CustId = 3,
                CTSCustId = 3,
                AccountStatus = "AccountStatus2",
                CategoryNames = "CategoryNames2",
                EvidenceCode = "EvidenceCode2",
                SiteId = 2,
                SiteName = "SiteName2",
                UserName = "UserName2"
            });

            return list;
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

        public IEnumerable<AssociationDetailModel> GetAssociationDetail(AssociationDetailFilterViewModel filter)
        {
            var list = new List<AssociationDetailModel>();

            list.Add(new AssociationDetailModel
            {
                SiteId = 1,
                SiteName = "12Bet",
                AccountStatus = "Open",
                AssociaionType = "IP",
                AssociatedDate = DateTime.Now,
                AssociatedDateString = DateTime.Now.ToShortDateString(),
                IsLinked = false
            });

            list.Add(new AssociationDetailModel
            {
                SiteId = 1,
                SiteName = "12Bet",
                AccountStatus = "Open",
                AssociaionType = "IP",
                AssociatedDate = DateTime.Now,
                AssociatedDateString = DateTime.Now.ToShortDateString(),
                IsLinked = true
            });

            return list;
        }
    }
}
