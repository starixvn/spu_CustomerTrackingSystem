namespace Fanex.NAP.CTS.Model.AssociationDetection
{
    using System;  

    public class GetAssociationDetectDetailMatchModel
    {
        public int LeagueID { get; set; }

        public int MatchID { get; set; }

        public int HomeID { get; set; }

        public int AwayID { get; set; }

        public DateTime EventDate { get; set; }

        public string UserName { get; set; }

        public int CustID { get; set; }

        public DateTime TransDate { get; set; }

        public long TransID { get; set; }

        public int BetType { get; set; }

        public string BetTeam { get; set; }

        public decimal Stake { get; set; }

        public decimal Odds { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

        public string TicketStatus { get; set; }      
    }
}
