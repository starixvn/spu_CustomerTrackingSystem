namespace Fanex.NAP.CTS.Model.Account
{
    using System;

    public class MatchesDetailInfoModel
    {
        public Int64 CustID { get; set; }

        public string UserName { get; set; }

        public DateTime TransDate { get; set; }

        public Int64 TransId { get; set; }

        public string TypeName { get; set; }

        public string BetTeam { get; set; }

        public decimal Stake { get; set; }
        
        public decimal Odds { get; set; }

        public decimal LiveHomeScore { get; set; }

        public decimal LiveAwayScore { get; set; }

        public decimal StakeRM { get; set; }

        public string Currency { get; set; }
        public string IP { get; set; }
    }
}
