namespace Fanex.NAP.CTS.ViewModel.DangerMonitor
{
    using System;

    public class GetDangerSummaryItemViewModel
    {
        public long MatchId { get; set; }

        public int LiveIndicator { get; set; }

        public int Bettype { get; set; }

        public int? BetID { get; set; }

        public int BetChoiceType { get; set; }

        public int SportType { get; set; }

        public int LeagueId { get; set; }

        public string LeagueName { get; set; }

        public DateTime EventDate { get; set; }

        public DateTime GlobalShowTime { get; set; }

        public int HomeId { get; set; }

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
    }
}