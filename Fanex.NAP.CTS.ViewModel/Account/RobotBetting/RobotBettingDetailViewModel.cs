namespace Fanex.NAP.CTS.ViewModel.Account.RobotBetting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RobotBettingDetailViewModel
    {
        public DateTime TransDate { get; set; }

        public string Match { get; set; }

        public long TransID { get; set; }

        public string BetType { get; set; }

        public string BetTeam { get; set; }

        public decimal Stake { get; set; }

        public decimal Odds { get; set; }

        public string LiveScore { get; set; }
    }
}
