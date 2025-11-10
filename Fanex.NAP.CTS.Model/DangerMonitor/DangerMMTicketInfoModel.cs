namespace Fanex.NAP.CTS.Model.DangerMonitor
{
    using Fanex.NAP.CTS.Model.DangerMonitor.Entities;
    using System;

    public class DangerMMTicketInfoModel
    {
        public DangerMMTicketInfoModel() { }

        public DangerMMTicketInfoModel(DangerMMTicketInfoEntity entity)
        {
            MatchSection = entity.MatchSection;
            EventDate = entity.EventDate;
            MatchId = entity.MatchId;
            LiveIndicator = entity.LiveIndicator ? 1 : 0;
            Bettype = entity.Bettype;
            BetID = entity.BetID;
            BetChoiceType = entity.BetChoiceType;
            LeagueId = entity.LeagueId;
            LeagueName = entity.LeagueName;
            SportType = entity.SportType;
            LeagueGroupID = entity.LeagueGroupID;
            GlobalShowTime = entity.GlobalShowTime;
            Homeid = entity.Homeid;
            HomeName = entity.HomeName;
            AwayId = entity.AwayId;
            AwayName = entity.AwayName;
            EventStatus = entity.EventStatus;
            LiveHomeScore = entity.LiveHomeScore;
            LiveAwayScore = entity.LiveAwayScore;
            TotalAmountHome = entity.TotalAmountHome;
            TotalAmountAway = entity.TotalAmountAway;
            TotalTicketHome = entity.TotalTicketHome;
            TotalTicketAway = entity.TotalTicketAway;
            ListCustID = entity.ListCustID;
        }

        public int MatchSection { get; set; }

        public DateTime EventDate { get; set; }

        public long MatchId { get; set; }

        public int LiveIndicator { get; set; }

        public int Bettype { get; set; }

        public int? BetID { get; set; }

        public int BetChoiceType { get; set; }

        public int LeagueId { get; set; }

        public string LeagueName { get; set; }

        public int SportType { get; set; }

        public int LeagueGroupID { get; set; }

        public DateTime GlobalShowTime { get; set; }

        public int Homeid { get; set; }

        public string HomeName { get; set; }

        public int AwayId { get; set; }

        public string AwayName { get; set; }

        public string EventStatus { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

        public long TotalAmountHome { get; set; }

        public long TotalAmountAway { get; set; }

        public int TotalTicketHome { get; set; }

        public int TotalTicketAway { get; set; }

        public string ListCustID { get; set; }
    }
}
