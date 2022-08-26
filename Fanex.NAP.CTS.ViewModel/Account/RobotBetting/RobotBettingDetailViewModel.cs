namespace Fanex.NAP.CTS.ViewModel.Account.RobotBetting
{
    public class RobotBettingDetailViewModel
    {
        public string TransDate { get; set; }        

        public string TransID { get; set; }

        public int BetType { get; set; }

        public string BetTypeName { get; set; }

        public string BetTeam { get; set; }

        public decimal Stake { get; set; }

        public decimal Odds { get; set; }

        public string LiveScore { get; set; }

        public int HomeID { get; set; }

        public int AwayID { get; set; }

        public int MatchId { get; set; }

        public string MatchName { get; set; }
    }
}
