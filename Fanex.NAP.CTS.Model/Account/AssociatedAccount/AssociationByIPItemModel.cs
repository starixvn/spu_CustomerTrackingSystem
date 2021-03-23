namespace Fanex.NAP.CTS.Model.Account
{
    using System;

    public class AssociationByIPItemModel
    {
        public int MatchID { get; set; }

        public int HomeID { get; set; }

        public int AwayID { get; set; }

        public int LeagueID { get; set; }

        public DateTime EventDate { get; set; }

        public string BettypeList { get; set; }

        public string IP { get; set; }
    }
}
