namespace Fanex.NAP.CTS.Model.ProblemAccountMonitor
{
    public class TWProblemAccountModel
    {

        public int CustID { get; set; }

        public string Username { get; set; }

        public decimal? Winloss { get; set; }

        public bool IsMonitorCategoryGroup { get; set; }

        public bool IsMonitorSite { get; set; }


    }
}
