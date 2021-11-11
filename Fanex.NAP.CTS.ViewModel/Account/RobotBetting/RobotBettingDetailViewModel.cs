namespace Fanex.NAP.CTS.ViewModel.Account.RobotBetting
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RobotBettingDetailViewModel
    {
        public DateTime TransDate { get; set; }        

        public long TransID { get; set; }

        public int BetType { get; set; }

        public string BetTeam { get; set; }

        public decimal Stake { get; set; }

        public decimal Odds { get; set; }

        public string LiveScore { get; set; }

        public int HomeID { get; set; }

        public int AwayID { get; set; }

        public string MatchName
        {
            get
            {
                var rs = "-";
                var HomeName = CachedRef.GetTeamName(HomeID);
                var AwayName = CachedRef.GetTeamName(AwayID);

                if (!string.IsNullOrEmpty(HomeName) || !string.IsNullOrEmpty(AwayName))
                {
                    rs = HomeName + " vs " + AwayName;
                }

                return rs;
            }
        }
    }
}
