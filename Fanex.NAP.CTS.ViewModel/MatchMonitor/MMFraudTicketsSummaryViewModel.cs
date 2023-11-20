namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using System;

    public class MMFraudTicketsSummaryViewModel
    {
        public Int64 MatchId { get; set; }

        public int IsLive { get; set; }

        public int Bettype { get; set; }

        public string Betteam { get; set; }

        public int? BetId { get; set; }

        public long Turnover { get; set; }

        public long BetCount { get; set; }

        public string BuildDictionaryKey()
        {
            var betIDToString = BetId != null ? BetId.ToString() : string.Empty;
            return $"{MatchId}_{IsLive}_{Bettype}_{Betteam}_{betIDToString}".ToLower();
        }
    }
}