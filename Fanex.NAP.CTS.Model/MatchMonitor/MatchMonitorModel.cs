namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;
    using System;

    public class MatchMonitorModel
    {
        public Int64 ID { get; set; }

        public Int64 MatchID { get; set; }

        public DateTime EventDate { get; set; }

        public int Market { get; set; }

        public int HomeID { get; set; }

        public int AwayID { get; set; }

        public int LeagueID { get; set; }

        public string LeagueName { get; set; }

        public int BettypeID { get; set; }

        public bool IsHedging { get; set; }

        public bool IsVerified { get; set; }

        public int MatchSection { get; set; }

        public string Reason
        {
            get
            {
                return this.IsHedging ? "Hedging" : "Group Betting";
            }
        }

        public string MarketName
        {
            get
            {
                return this.Market == 1 ? "Live" : "Non Live";
            }
        }

        public string EventDateToString
        {
            get
            {
                if (EventDate == DateTime.MinValue)
                {
                    return "-";
                }

                return EventDate.ToString(FormatDateToString_MMMddyyyy);
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

        public string ReasonGroup
        {
            get
            {
                switch (BettypeID)
                {
                    case (int)MATCHMONITOR_BETTYPE.HDP:
                        return Reason + " - HDP";
                    case (int)MATCHMONITOR_BETTYPE.OU:
                        return Reason + " - O/U";
                    case (int)MATCHMONITOR_BETTYPE.FirstHDP:
                        return Reason + " - 1st HDP";
                    case (int)MATCHMONITOR_BETTYPE.FirsttOU:
                        return Reason + " - 1st O/U";
                }

                return Reason;
            }
        }

        public string MatchSectionName
        {
            get
            {
                switch (MatchSection)
                {
                    case (int)MATCHMONITOR_SECTION.Early:
                        return Enum.GetName(typeof(MATCHMONITOR_SECTION), MATCHMONITOR_SECTION.Early);
                    case (int)MATCHMONITOR_SECTION.Live:
                        return Enum.GetName(typeof(MATCHMONITOR_SECTION), MATCHMONITOR_SECTION.Live);
                    case (int)MATCHMONITOR_SECTION.Today:
                        return Enum.GetName(typeof(MATCHMONITOR_SECTION), MATCHMONITOR_SECTION.Today);
                    case (int)MATCHMONITOR_SECTION.Closed:
                        return Enum.GetName(typeof(MATCHMONITOR_SECTION), MATCHMONITOR_SECTION.Closed);
                }

                return "";
            }
        }
    }
}
