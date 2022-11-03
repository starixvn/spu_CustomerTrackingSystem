namespace Fanex.NAP.CTS.ViewModel.AssociatedGroupMonitor
{
    using Fanex.NAP.CTS.Model.AssociatedGroupMonitor;
    using Fanex.NAP.CTS.ViewModel.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class GetProblemCategoryByGroupIDViewModel
    {
        public GetProblemCategoryByGroupIDViewModel() { }

        public GetProblemCategoryByGroupIDViewModel(GetProblemCategoryByGroupIDModel data)
        {
            this.GroupID = data.GroupID;
            this.PACategoryID = data.PACategoryID.HasValue ? data.PACategoryID.Value : -1;
            this.PAAllCredit = data.PAAllCredit;
            this.PAAllLicensee = data.PAAllLicensee;
            this.PASites = data.PASites;
            this.ModifiedBy = data.ModifiedBy;
            this.SiteGroups = Enumerable.Empty<SiteGroupViewModel>();
        }

        public long GroupID { get; set; }

        public int PACategoryID { get; set; }

        public int PAAllCredit { get; set; }

        public int PAAllLicensee { get; set; }

        public string PASites { get; set; }

        public string ModifiedBy { get; set; }

        public IEnumerable<SiteGroupViewModel> SiteGroups { get; set; }

        public void AddSiteGroups(IEnumerable<AssociatedGroupSiteModel> sites)
        {
            if (sites != null && sites.Any())
            {
                var paSiteList = new List<int>();

                if (!string.IsNullOrEmpty(this.PASites))
                {
                    paSiteList = this.PASites.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                }
                var siteGroups = sites.Where(x => x.IsChosen).OrderBy(x => x.IsLicensee)
                             .GroupBy(g => g.IsLicensee)
                             .Select(x => new { SubscriberGroupName = x.Key ? "Licensees" : "Credit", SiteList = x.ToList() });

                if (this.PACategoryID != -1)
                {
                    this.SiteGroups = siteGroups.Select(x => new SiteGroupViewModel
                             {
                                 SubscriberGroupName = x.SubscriberGroupName,
                                 SiteList = x.SiteList.Select(y =>
                                 {
                                     return new SiteViewModel
                                     {
                                         SiteID = y.SiteID,
                                         SiteName = y.SiteName,
                                         IsSelected = paSiteList.Any(a => a == y.SiteID)
                                     };
                                 })
                             });
                } else {
                    this.SiteGroups = siteGroups.Select(x => new SiteGroupViewModel
                             {
                                 SubscriberGroupName = x.SubscriberGroupName,
                                 SiteList = x.SiteList.Select(y => new SiteViewModel
                                 {
                                     SiteID = y.SiteID,
                                     SiteName = y.SiteName,
                                     IsSelected = true
                                 })
                             });
                }
            }            
        }
    }
}
