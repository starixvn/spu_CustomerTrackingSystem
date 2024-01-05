namespace Fanex.NAP.CTS.ViewModel.DangerMonitor
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Resource.Consts;
    using System;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class DangerMonitorInfoViewModel
    {
        public DangerMonitorInfoViewModel(MainMatchModel mainMatchModel, IDictionary<int, string> teamNameDic, IDictionary<int, string> leagueNameDic)
        {
            MatchID = mainMatchModel.MatchID;
            HomeID = mainMatchModel.HomeId;
            AwayID = mainMatchModel.AwayId;
            LeagueID = mainMatchModel.LeagueId;
            SportType = mainMatchModel.SportType;
            EventDate = mainMatchModel.EventDate;
            MatchSection = mainMatchModel.MatchSection;
            KickOffTime = mainMatchModel.KickOffTime;

            if (teamNameDic.TryGetValue(HomeID, out string homeName))
            {
                HomeName = homeName;
            }
            if (teamNameDic.TryGetValue(AwayID, out string awayName))
            {
                AwayName = awayName;
            }
            if (leagueNameDic.TryGetValue(LeagueID, out string leagueName))
            {
                LeagueName = leagueName;
            }
        }

        public DangerMonitorInfoViewModel(DangerMonitorInfoViewModel matchInfo, DangerMonitorTicketSummaryViewModel matchSummary)
        {
            MatchID = matchInfo.MatchID;

            TotalAmtHome = matchSummary.TotalAmountHome;
            TotalAmtAway = matchSummary.TotalAmountAway;
            TotalTicketHome = matchSummary.TotalTicketHome;
            TotalTicketAway = matchSummary.TotalTicketAway;
            Market = matchSummary.LiveIndicator;
            BettypeID = matchSummary.Bettype;
            BetID = matchSummary.BetID;
            BetChoiceType = matchSummary.BetChoiceType;

            HomeID = matchInfo.HomeID;
            HomeName = matchInfo.HomeName;
            AwayID = matchInfo.AwayID;
            AwayName = matchInfo.AwayName;
            LeagueID = matchInfo.LeagueID;
            LeagueName = matchInfo.LeagueName;
            SportType = matchInfo.SportType;
            EventDate = matchInfo.EventDate;
            MatchSection = matchInfo.MatchSection;
            KickOffTime = matchInfo.KickOffTime;
        }

        public int BetChoiceType { get; set; }

        public long MatchID { get; set; }

        public int AwayID { get; set; }

        public string AwayName { get; set; }

        public int HomeID { get; set; }

        public string HomeName { get; set; }

        public int LeagueID { get; set; }

        public string LeagueName { get; set; }

        public int Market { get; set; }

        public DateTime EventDate { get; set; }

        public int SportType { get; set; }

        public string SportName { get; set; }

        public int MatchSection { get; set; }

        public int BettypeID { get; set; }

        public string BettypeName { get; set; }

        public int? BetID { get; set; }

        public long TotalAmtHome { get; set; }

        public long TotalAmtAway { get; set; }

        public int TotalTicketHome { get; set; }

        public int TotalTicketAway { get; set; }

        public DateTime KickOffTime { get; set; }

        public string KOTime { get { return KickOffTime.ToString(Consts.FormatTimeToString_hhmmss_2); } }

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

        public void MappingTicketSummary(DangerMonitorTicketSummaryViewModel monitorSummary)
        {
            MatchID = monitorSummary.MatchId;

            TotalAmtHome = monitorSummary.TotalAmountHome;
            TotalAmtAway = monitorSummary.TotalAmountAway;
            TotalTicketHome = monitorSummary.TotalTicketHome;
            TotalTicketAway = monitorSummary.TotalTicketAway;
            Market = monitorSummary.LiveIndicator;
            BettypeID = monitorSummary.Bettype;
            BetChoiceType = monitorSummary.BetChoiceType;
        }

        public void MappingBettypeSportInfo(DangerMonitorBetTypeViewModel sportBettype)
        {
            BettypeName = sportBettype.BetTypeName;
            BetID = sportBettype.BetID;
            SportName = sportBettype.SportName;
        }
    }
}
