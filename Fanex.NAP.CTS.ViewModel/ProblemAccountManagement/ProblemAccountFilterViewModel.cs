namespace Fanex.NAP.CTS.ViewModel.ProblemAccountManagement
{
    using System;   

    public class ProblemAccountFilterViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Sites { get; set; }
        
        public bool IsAllSites { get; set; }

        public string CreditSiteNames { get; set; }

        public string LicenseeSiteNames { get; set; }

        public string CN88SiteNames { get; set; }

        public string CategoryIDs { get; set; }

        public bool IsAllCategories { get; set; }

        public string CategoryNames { get; set; }

        public string UserName { get; set; }

        public string RoleIDs { get; set; }

        public bool HasDownline { get; set; }

        public DateTime DateTime { get; set; }
    }
}
