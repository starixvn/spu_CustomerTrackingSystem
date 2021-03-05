namespace Fanex.NAP.CTS.Model.Account
{
    using System;

    public class MatchesInfoModel
    {
        public int MatchID { get; set; }

        public string BettypeList { get; set; }

        public DateTime EventDate { get; set; }

        public int HomeId { get; set; }

        public int AwayId { get; set; }

        public int LeagueId { get; set; }

        public string HomeName { get; set; }

        public string AwayName { get; set; }

        public string LeagueName { get; set; }
    }
}
