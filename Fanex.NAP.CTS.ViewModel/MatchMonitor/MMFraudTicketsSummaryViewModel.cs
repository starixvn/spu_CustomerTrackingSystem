namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using System;

    public class MMFraudTicketsSummaryViewModel
    {
        public MMFraudTicketsSummaryViewModel() { }

        public MMFraudTicketsSummaryViewModel(MMFraudTicketsSummaryEntity entity)
        {
            MatchId = entity.MatchId;
            IsLive = entity.IsLive ? 1 : 0;
            Bettype = entity.Bettype;
            Betteam = entity.Betteam;
            BetId = entity.BetId;
            Turnover = entity.Turnover;
            BetCount = entity.BetCount;
        }

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