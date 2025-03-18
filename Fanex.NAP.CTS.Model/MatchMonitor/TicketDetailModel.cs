namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using Fanex.NAP.CTS.Model.MatchMonitor.Parlay;
    using System;
    using System.Collections.Generic;

    public class TicketDetailModel
    {
        public long SequenceID { get; set; }

        public Int64 TransId { get; set; }

        public DateTime TransDate { get; set; }

        public Int64 MatchId { get; set; }

        public Int64 HomeId { get; set; }

        public string HomeName { get; set; }

        public Int64 AwayId { get; set; }

        public string AwayName { get; set; }

        public Int64 LeagueId { get; set; }

        public string LeagueName { get; set; }

        public DateTime EventDate { get; set; }

        public string EventStatus { get; set; }

        public DateTime KickOffTime { get; set; }

        public int BetType { get; set; }

        public long BetID { get; set; }

        public Int64 CustId { get; set; }

        public string BettypeName { get; set; }

        public string SportType { get; set; } // SportName

        public string BetTeam { get; set; }

        public string BetTeamFullName { get; set; }

        public string BetFrom { get; set; }

        public Decimal Stake { get; set; }

        public Decimal OrgStake { get; set; }

        public Decimal Odds { get; set; }

        public Decimal? MalayOdds { get; set; }

        public Decimal Hdp1 { get; set; }

        public Decimal Hdp2 { get; set; }

        public string Choice { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

        public bool LiveIndicator { get; set; }

        public int TicketStatus { get; set; }

        public int Danger1 { get; set; }

        public int Danger2 { get; set; }

        public int Danger3 { get; set; }

        public int Danger4 { get; set; }

        public int Danger5 { get; set; }

        public int CustomerClass { get; set; }

        public string CurrencyName { get; set; }

        public Int16 CurrencyId { get; set; }

        public Decimal Hdp { get; set; }

        public int IsLicensee { get; set; }

        public int ChoiceOrder { get; set; }

        public int TicketOrder { get; set; }

        public long Refno { get; set; }

        public int Combinition { get; set; }

        public IEnumerable<ParlayTicketInfoModel> ParlayTickets { get; set; }
    }
}