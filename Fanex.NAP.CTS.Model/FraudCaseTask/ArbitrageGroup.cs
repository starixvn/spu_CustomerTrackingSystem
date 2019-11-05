namespace Fanex.NAP.CTS.Library.Models.FraudCaseTask
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using System;

    [XName("Match")]
    public class ArbitrageGroup
    {
        public int MatchId { get; set; }

        [XName("BetType")]
        public int Bettype { get; set; }

        public int TotalScore { get; set; }

        public double Handicap { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as ArbitrageGroup;

            if(other == null)
            {
                return false;
            }

            return MatchId == other.MatchId
                && Bettype == other.Bettype
                && TotalScore == other.TotalScore
                && Handicap == other.Handicap;
        }

        public override int GetHashCode()
        {
            var t = Tuple.Create(MatchId, Bettype, TotalScore, Handicap);

            return t.GetHashCode();
        }
    }
}