namespace Fanex.NAP.CTS.Library.Models.FraudCaseTask
{
    using Fanex.NAP.CTS.Library.Core.Helpers.Xml;

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