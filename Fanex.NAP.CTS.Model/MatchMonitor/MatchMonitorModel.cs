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

        public int? BetID { get; set; }

        public int Reason { get; set; }

        public int SportType { get; set; }

        public int MatchSection { get; set; }

        public bool IsVerified { get; set; }

        public string ReasonName
        {
            get
            {
                switch (Reason)
                {
                    case 0:
                        return "Group Betting";
                    case 1:
                        return "Hedging";
                    case 2:
                        return "Arbitrage";
                    case 3:
                        return "Fixed Game";
                }

                return string.Empty;
            }
        }

        public string MarketName
        {
            get
            {
                return this.Market == 1 ? "Live" : "Non-Live";
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
                    case (int)MatchMonitorBetType.HDP:
                        return ReasonName + " - " + MatchMonitorBetType_HDP;
                    case (int)MatchMonitorBetType.OU:
                        return ReasonName + " - " + MatchMonitorBetType_OU;
                    case (int)MatchMonitorBetType.FirstHDP:
                        return ReasonName + " - " + MatchMonitorBetType_1stHDP;
                    case (int)MatchMonitorBetType.FirsttOU:
                        return ReasonName + " - " + MatchMonitorBetType_1stOU;         
                    case (int)MatchMonitorBetType.Moneyline:
                        return ReasonName + " - " + MatchMonitorBetType_Moneyline;
                    case (int)MatchMonitorBetType.QuarterXHDP:
                        {
                            switch(BetID)
                            {
                                case (int)MatchMonitorBetId.first:
                                    return ReasonName + " - " + MatchMonitorBetType_Quarter1HDP; 
                                case (int)MatchMonitorBetId.second:
                                    return ReasonName + " - " + MatchMonitorBetType_Quarter2HDP; 
                                case (int)MatchMonitorBetId.third:
                                    return ReasonName + " - " + MatchMonitorBetType_Quarter3HDP;
                                case (int)MatchMonitorBetId.fourth:
                                    return ReasonName + " - " + MatchMonitorBetType_Quarter4HDP;
                            }

                            return ReasonName + " - " + MatchMonitorBetType_QuarterXHDP;
                        }
                    case (int)MatchMonitorBetType.QuarterXOU:
                        {
                            switch (BetID)
                            {
                                case (int)MatchMonitorBetId.first:
                                    return ReasonName + " - " + MatchMonitorBetType_Quarter1OU;
                                case (int)MatchMonitorBetId.second:
                                    return ReasonName + " - " + MatchMonitorBetType_Quarter2OU;
                                case (int)MatchMonitorBetId.third:
                                    return ReasonName + " - " + MatchMonitorBetType_Quarter3OU;
                                case (int)MatchMonitorBetId.fourth:
                                    return ReasonName + " - " + MatchMonitorBetType_Quarter4OU;
                            }

                            return ReasonName + " - " + MatchMonitorBetType_QuarterXOU;

                        }
                    case (int)MatchMonitorBetType.QuarterXMoneyline:
                        {
                            switch (BetID)
                            {
                                case (int)MatchMonitorBetId.first:
                                    return ReasonName + " - " + MatchMonitorBetType_Quarter1Moneyline;
                                case (int)MatchMonitorBetId.second:
                                    return ReasonName + " - " + MatchMonitorBetType_Quarter2Moneyline;
                                case (int)MatchMonitorBetId.third:
                                    return ReasonName + " - " + MatchMonitorBetType_Quarter3Moneyline;
                                case (int)MatchMonitorBetId.fourth:
                                    return ReasonName + " - " + MatchMonitorBetType_Quarter4Moneyline;
                            }

                            return ReasonName + " - " + MatchMonitorBetType_QuarterXMoneyline;
                        }
                }

                return string.Empty;
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

        public string SportName
        {
            get
            {
                switch (this.SportType)
                {
                    case (int)MatchMonitorSportType.Soccer:
                        return nameof(MatchMonitorSportType.Soccer);
                    case (int)MatchMonitorSportType.Basketball:
                        return nameof(MatchMonitorSportType.Basketball); 
                    case (int)MatchMonitorSportType.ESport:
                        return "E-Sports";
                }

                return string.Empty;
            }
        }
    }
}
