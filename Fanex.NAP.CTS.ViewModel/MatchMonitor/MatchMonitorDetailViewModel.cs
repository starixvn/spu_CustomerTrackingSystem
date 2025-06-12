namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Model;
    using System;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class MatchMonitorDetailViewModel
    {
        public string EventDateToString { get; set; }

        public Int64 MatchID { get; set; }

        public int HomeID { get; set; }

        public int AwayID { get; set; }

        public int BetTypeID { get; set; }

        public string BettypeName { get; set; }

        public Int64 BetID { get; set; }

        public int MatchSection { get; set; }

        public int Market { get; set; }

        public int Reason { get; set; }

        public string ReasonName { get; set; }

        public int LeagueID { get; set; }
        
        public int SportType { get; set; }

        public string KOTime { get; set; }

        public bool IsSupportShowAllTickets { get; set; }

        public int MX { get; set; }

        public int MY { get; set; }

        public bool LiveIndicator
        {
            get
            {
                return Market == 1;
            }
        }

        public string HomeName
        {
            get
            {
                //return CachedRef.GetTeamName(HomeID); RemotingRef not working
                return HomeID.ToString();
            }
        }

        public string AwayName
        {
            get
            {
                //return CachedRef.GetTeamName(AwayID); RemotingRef not working
                return AwayID.ToString();
            }
        }

        public string LeagueName
        {
            get
            {
                //return CachedRef.GetLeagueName(LeagueID).ToLower(); RemotingRef not working
                return LeagueID.ToString().ToLower(); // Assuming LeagueName is directly available in the model
            }
        }

        public string MarketName
        {
            get
            {
                return Market == 1 ? "Live" : "Non Live";
            }
        }
        
        public string ReasonGroup
        {
            get
            {
                switch (BetTypeID)
                {
                    case (int)MatchMonitorBetType.HDP:
                        return ReasonName + " - " + MatchMonitorBetType_HDP;

                    case (int)MatchMonitorBetType.OU:
                        return ReasonName + " - " + MatchMonitorBetType_OU;

                    case (int)MatchMonitorBetType.FirstHDP:
                        return ReasonName + " - " + MatchMonitorBetType_1stHDP;

                    case (int)MatchMonitorBetType.FirstOU:
                        return ReasonName + " - " + MatchMonitorBetType_1stOU;

                    case (int)MatchMonitorBetType.Moneyline:
                        return ReasonName + " - " + MatchMonitorBetType_Moneyline;

                    case (int)MatchMonitorBetType.QuarterXHDP:
                        {
                            switch (BetID)
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

        public List<MatchMonitorBetChoiceId> BetChoice
        {
            get
            {
                var betchoiceOption1 = new List<MatchMonitorBetChoiceId> { MatchMonitorBetChoiceId.All, MatchMonitorBetChoiceId.H, MatchMonitorBetChoiceId.A };

                var betchoiceOption2 = new List<MatchMonitorBetChoiceId> { MatchMonitorBetChoiceId.All, MatchMonitorBetChoiceId.O, MatchMonitorBetChoiceId.U };

                switch (BetTypeID)
                {
                    case (int)MatchMonitorBetType.HDP:
                    case (int)MatchMonitorBetType.Moneyline:
                    case (int)MatchMonitorBetType.FirstHDP:
                        return betchoiceOption1;

                    case (int)MatchMonitorBetType.QuarterXHDP:
                        {
                            switch (BetID)
                            {
                                case (int)MatchMonitorBetId.first:
                                case (int)MatchMonitorBetId.second:
                                case (int)MatchMonitorBetId.third:
                                case (int)MatchMonitorBetId.fourth:
                                    return betchoiceOption1;
                            }

                            return new List<MatchMonitorBetChoiceId>();
                        }
                    case (int)MatchMonitorBetType.QuarterXMoneyline:
                        {
                            switch (BetID)
                            {
                                case (int)MatchMonitorBetId.first:
                                case (int)MatchMonitorBetId.second:
                                case (int)MatchMonitorBetId.third:
                                case (int)MatchMonitorBetId.fourth:
                                    return betchoiceOption1;
                            }

                            return new List<MatchMonitorBetChoiceId>();
                        }
                    case (int)MatchMonitorBetType.OU:
                    case (int)MatchMonitorBetType.FirstOU:
                        return betchoiceOption2;

                    case (int)MatchMonitorBetType.QuarterXOU:
                        {
                            switch (BetID)
                            {
                                case (int)MatchMonitorBetId.first:
                                case (int)MatchMonitorBetId.second:
                                case (int)MatchMonitorBetId.third:
                                case (int)MatchMonitorBetId.fourth:
                                    return betchoiceOption2;
                            }

                            return new List<MatchMonitorBetChoiceId>();
                        }
                }

                return new List<MatchMonitorBetChoiceId>();
            }
        }

        public string SportName
        {
            get
            {
                if (SportNameDictonary.TryGetValue(SportType, out var sportName))
                {
                    return sportName;
                }

                return string.Empty;
            }
        }

        public TicketType TicketType { get; set; }
    }
}