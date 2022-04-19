namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using System;

    public class MatchesDetailInfoViewModel
    {
        public Int64 GroupID{ get; set; }

        public string Description { get; set; }

        public string UserName { get; set; }

        public string TransDateString { get; set; }

        public Int64 TransId { get; set; }

        public string BetType { get; set; }

        public string BetTeam { get; set; }

        public decimal Stake { get; set; }

        public decimal Odds { get; set; }

        public string LiveScore { get; set; }
    }
}
