namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class GetMatchMonitorViewModel
    {
        public GetMatchMonitorViewModel(GetMatchMonitorModel data, MatchMonitorStaticListModel mmStatic)
        {            
            this.ID = data.ID;
            this.MatchID = data.MatchID;
            this.EventDate = data.EventDate;
            this.Market = data.Market;
            this.HomeID = data.HomeID;
            this.HomeName = CachedRef.GetTeamName(data.HomeID);
            this.AwayID = data.AwayID;
            this.AwayName = CachedRef.GetTeamName(data.AwayID);
            this.LeagueID = data.LeagueID;
            this.LeagueName = CachedRef.GetLeagueName(data.LeagueID);     
            this.BettypeID = data.BettypeID;
            this.BettypeName = this.GetBettypeName(mmStatic.BetTypes, data.BettypeID, data.BetID);
            this.BetID = data.BetID;
            this.Reason = data.Reason;
            this.ReasonName = this.GetReasonName(mmStatic.Reasons, data.Reason);
            this.SportType = data.Sporttype;
            this.MatchSection = data.MatchSection;
            this.IsVerified = data.IsVerified;
            this.VerifiedBy = data.VerifiedBy;
            this.KOTime = data.KickOffTime.ToString(Consts.FormatTimeToString_hhmmss_2);
            this.IsSupportShowAllTickets = data.IsSupportShowAllTickets;
        }

        public Int64 ID { get; set; }

        public Int64 MatchID { get; set; }

        public DateTime EventDate { get; set; }

        public int Market { get; set; }

        public int HomeID { get; set; }

        public string HomeName { get; set; }

        public long HomeTotalAmount { get; set; }

        public long HomeBetCount { get; set; }

        public int AwayID { get; set; }

        public string AwayName { get; set; }

        public long AwayTotalAmount { get; set; }

        public long AwayBetCount { get; set; }

        public int LeagueID { get; set; }

        public string LeagueName { get; set; }

        public int BettypeID { get; set; }

        public string BettypeName { get; set; }

        public int? BetID { get; set; }

        public string Reason { get; set; }

        public string ReasonName { get; set; }

        public int SportType { get; set; }

        public int MatchSection { get; set; }

        public bool IsVerified { get; set; }

        public string VerifiedBy { get; set; }

        public string KOTime { get; set; }

        public bool IsSupportShowAllTickets { get; set; }

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
                
                if (!string.IsNullOrEmpty(this.HomeName) || !string.IsNullOrEmpty(this.AwayName))
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

        private string GetBettypeName(IEnumerable<MatchMonitorBetTypeModel> betTypes, int betTypeID, int? betID)
        {
            return betTypes.FirstOrDefault(x => x.BetTypeID == betTypeID && x.BetID == betID)?.BetTypeName;
        }

        private string GetReasonName(IEnumerable<MatchMonitorReasonModel> Reasons, string reason)
        {
            if (!string.IsNullOrEmpty(reason))
            {
                var arrReasons = reason.Split(',').Select(x => Convert.ToInt32(x));
                var reasonNames = Reasons.Join(arrReasons,
                           r1 => r1.ReasonID,
                           r2 => r2,
                           (r1, r2) => r1.ReasonName);

                return string.Join(",", reasonNames);
            }

            return string.Empty;
        }

        public string BuildTicketsSummaryDictionaryKey(string betteam)
        {
            var betIDToString = BetID != null ? BetID.ToString() : string.Empty;
            return $"{MatchID}_{Market}_{BettypeID}_{betteam}_{betIDToString}".ToLower();
        }
    }
}
