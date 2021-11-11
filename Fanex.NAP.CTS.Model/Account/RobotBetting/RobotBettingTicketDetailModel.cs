namespace Fanex.NAP.CTS.Model.Account.RobotBetting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RobotBettingTicketDetailModel
    {
        public long TransId { get; set; }

        public DateTime TransDate { get; set; }

        public int MatchId { get; set; }

        public DateTime EventDate { get; set; }

        public DateTime KickOffTime { get; set; }

        public int HomeId { get; set; }

        public int AwayId { get; set; }

        public int LeagueId { get; set; }

        public int BetType { get; set; }

        public int CustId { get; set; }

        public string BetTeam { get; set; }

        public decimal Stake { get; set; }

        public decimal Odds { get; set; }

        public decimal Hdp1 { get; set; }

        public decimal Hdp2 { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

        public int StatusID { get; set; }       
       
        public bool LiveIndicator { get; set; }        
    }
}
