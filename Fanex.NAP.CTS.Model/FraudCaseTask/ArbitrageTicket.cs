namespace Fanex.NAP.CTS.Model.FraudCaseTask
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;

    [XName("Match")]
    public class ArbitrageTicket
    {
        public int MatchId { get; set; }

        public int CustId { get; set; }

        [XName("BetType")]
        public int Bettype { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

        public double Handicap { get; set; }
    }
}