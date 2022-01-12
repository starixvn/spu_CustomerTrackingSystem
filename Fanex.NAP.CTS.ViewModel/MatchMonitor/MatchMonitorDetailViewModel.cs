namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using System;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class MatchMonitorDetailViewModel
    {
        public string EventDateToString { get; set; }

        public Int64 MatchID { get; set; }

        public int HomeID { get; set; }

        public int AwayID { get; set; }

        public int BetTypeID { get; set; }

        public int MatchSection { get; set; }

        public int Market { get; set; }

        public bool IsHedging { get; set; }

        public int LeagueID { get; set; }

        public bool LiveIndicator
        {
            get
            {
                return Market == 1;
            }
        }

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

        public string LeagueName
        {
            get
            {
                return CachedRef.GetLeagueName(LeagueID);
            }
        }

        public string MarketName
        {
            get
            {
                return Market == 1 ? "Live" : "Non Live";
            }
        }

        public string Reason
        {
            get
            {
                return this.IsHedging ? "Hedging" : "Group Betting";
            }
        }

        public string ReasonName
        {
            get
            {
                switch (BetTypeID)
                {
                    case (int)MatchMonitorBetType.HDP:
                        return Reason + " - HDP";
                    case (int)MatchMonitorBetType.OU:
                        return Reason + " - O/U";
                    case (int)MatchMonitorBetType.FirstHDP:
                        return Reason + " - 1st HDP";
                    case (int)MatchMonitorBetType.FirsttOU:
                        return Reason + " - 1st O/U";
                }

                return Reason;
            }
        }
    }
}