namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using Fanex.NAP.CTS.Model.MatchMonitor.Entities;
    using System;

    public class GetMatchMonitorModel
    {
        public GetMatchMonitorModel() { }

        public GetMatchMonitorModel(GetMatchMonitorEntity entity)
        {
            ID = entity.ID;
            MatchID = entity.MatchID;
            EventDate = entity.EventDate;
            Market = entity.Market;
            HomeID = entity.HomeID;
            HomeName = entity.HomeName;
            AwayID = entity.AwayID;
            AwayName = entity.AwayName;
            LeagueID = entity.LeagueID;
            LeagueName = entity.LeagueName;
            Sporttype = entity.SportTypeId;
            BettypeID = entity.BettypeID;
            BetID = entity.BetID;
            Reason = entity.Reason;
            MatchSection = entity.MatchSection;
            IsVerified = entity.IsVerified;
            VerifiedBy = entity.VerifiedBy;
            KickOffTime = entity.KickOffTime;
            IsSupportShowAllTickets = entity.IsSupportShowAllTickets;
            FraudTrans = entity.FraudTrans;
            FraudRefno = entity.FraudRefno;
            DataSource = entity.DataSource;
            BettypeNameDisplay = entity.BettypeNameDisplay;
            DetailsX = entity.DetailsX;
            DetailsY = entity.DetailsY;
            TicketType = entity.TicketType; // 1: Single, 2: Parlay
        }

        public Int64 ID { get; set; }

        public Int64 MatchID { get; set; }

        public DateTime EventDate { get; set; }

        public int Market { get; set; }

        public int HomeID { get; set; }

        public string HomeName { get; set; }

        public int AwayID { get; set; }

        public string AwayName { get; set; }

        public int LeagueID { get; set; }

        public string LeagueName { get; set; }

        public int Sporttype { get; set; }

        public int BettypeID { get; set; }

        public int? BetID { get; set; }

        public string Reason { get; set; }

        public int MatchSection { get; set; }

        public bool IsVerified { get; set; }

        public string VerifiedBy { get; set; }

        public DateTime KickOffTime { get; set; }

        public bool IsSupportShowAllTickets { get; set; }

        public string FraudTrans { get; set; }

        public string FraudRefno { get; set; }

        public short DataSource { get; set; }

        public string BettypeNameDisplay { get; set; }

        public int DetailsX { get; set; }

        public int DetailsY { get; set; }

        public int TicketType { get; set; } // 1: Single, 2: Parlay
    }
}
