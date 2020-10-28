namespace Fanex.NAP.CTS.Service
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.NAP.CTS.Criteria.AssociationReport;
    using Fanex.NAP.CTS.Model.AssociationReport;
    using Fanex.NAP.CTS.ViewModel.AssociationReport;

    public class AssociationReportService : IAssociationReportService
    {
        public IEnumerable<AssociationModel> GetAssociations(AssociationFilterViewModel filter)
        {
            var list = new List<AssociationModel>();

            list.Add(new AssociationModel
            {
                CustId = 1,
                CTSCustId = 1,
                UserName = "AXN122346",
                AccountStatus = "Open",
                CategoryNames = "Normal Account",
                EvidenceCodes = "6.1",
                SiteId = 1,
                SiteName = "12Bet",
            });

            list.Add(new AssociationModel
            {
                CustId = 3,
                CTSCustId = 3,
                UserName = "AXM122789",
                AccountStatus = "Open",
                CategoryNames = "Normal Account",
                EvidenceCodes = "6.2",
                SiteId = 1,
                SiteName = "12Bet",
            });

            return list;
        }

        public IEnumerable<CategoriesModel> GetCategories(string ctsCustIdList)
        {
            var criteria = new AssociationReportGetCategoriesCriteria
            {
                CTSCustIdList = ctsCustIdList
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CategoriesModel>(criteria);
            }
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
