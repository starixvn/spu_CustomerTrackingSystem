namespace Fanex.NAP.CTS.Model.ProblemAccountMonitor
{
    public class TWProblemAccountModel
    {

        public int CustID { get; set; }

        public string Username { get; set; }

        public int RoleID { get; set; }

        public int CustomerClass { get; set; }

        public decimal? Winloss { get; set; }

        public decimal Turnover { get; set; }

        public int BetCount { get; set; }

        public int ActiveDays { get; set; }


    }
}
