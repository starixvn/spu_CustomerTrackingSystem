namespace Fanex.NAP.CTS.Model.Account
{
    using System;

    public class MatchesInfoModel
    {
        public int MatchID { get; set; }

        public string BettypeList { get; set; }

        public DateTime EventDate { get; set; }

        public int HomeID { get; set; }

        public int AwayID { get; set; }

        public int LeagueID { get; set; }

        public string HomeName { get; set; }

        public string AwayName { get; set; }

        public string LeagueName { get; set; }
    }
}
