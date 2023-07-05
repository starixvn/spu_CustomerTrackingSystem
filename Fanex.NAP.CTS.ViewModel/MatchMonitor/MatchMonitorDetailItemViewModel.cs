namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class MatchMonitorDetailItemViewModel
    {
        #region constructor
        public MatchMonitorDetailItemViewModel() { }

        public MatchMonitorDetailItemViewModel(TicketDetailModel ticket)
        {
            MappingTicketDetail(ticket);
        }

        public MatchMonitorDetailItemViewModel(TicketDetailModel ticket, CustInfoModel cust)
        {
            MappingTicketDetail(ticket);
            MappingCustInfo(cust);
        }

        public MatchMonitorDetailItemViewModel(int mergeGroupID, TicketDetailModel ticket, CustInfoModel cust)
        {
            MappingTicketDetail(ticket);
            MappingCustInfo(cust);
            this.ADetectGroup = mergeGroupID.ToString();
            this.ADetectGroupID = mergeGroupID;
        }
        #endregion

        #region property
        public Int64 CTSCustID { get; set; }

        public Int64 CustID { get; set; }

        public string SubscriberType { get; set; }

        public string Username { get; set; }

        public int CustomerClass { get; set; }

        public int Danger1 { get; set; }

        public int Danger2 { get; set; }

        public int Danger3 { get; set; }

        public int Danger4 { get; set; }

        public int Danger5 { get; set; }

        public string DangerToString
        {
            get
            {
                string danger = string.Empty;

                if (Danger1 != 0)
                    danger += "Ori" + Danger1;

                if (Danger2 != 0)
                    danger += string.IsNullOrEmpty(danger) ? "ABI" + Danger2 : "-ABI" + Danger2;

                if (Danger3 != 0)
                    danger += string.IsNullOrEmpty(danger) ? "OS" + Danger3 : "-OS" + Danger3;

                if (Danger4 != 0)
                    danger += string.IsNullOrEmpty(danger) ? "BkB" + Danger4 : "-BkB" + Danger4;

                if (Danger5 != 0)
                    danger += string.IsNullOrEmpty(danger) ? "ES" + Danger5 : "-ES" + Danger5;

                return (danger == string.Empty) ? "-" : danger;
            }
        }

        public decimal Odds { get; set; }

        public string BetID { get; set; }

        public string BetTeam { get; set; }

        public string CurrencyName { get; set; }

        public short CurrencyId { get; set; }

        public decimal Stake { get; set; }

        public decimal Amount { get; set; }

        public decimal HDP1 { get; set; }

        public decimal HDP2 { get; set; }

        public decimal HDP
        {
            get
            {
                if (BetTypeID == (int)MatchMonitorBetType.OU
                    || BetTypeID == (int)MatchMonitorBetType.FirsttOU
                    || BetTypeID == (int)MatchMonitorBetType.QuarterXOU)
                {
                    return HDP1;
                }

                if (!string.IsNullOrEmpty(BetTeam))
                {
                    if (string.Equals(BetTeam, "h", StringComparison.OrdinalIgnoreCase))
                    {
                        return HDP2 - HDP1;
                    }
                    else if (string.Equals(BetTeam, "a", StringComparison.OrdinalIgnoreCase))
                    {
                        return HDP1 - HDP2;
                    }
                }

                return 0;
            }
        }

        public string Score { get; set; }

        public int TicketStatus { get; set; }

        public DateTime TransTime { get; set; }

        public string TransTimeToString { get; set; }

        public string ADetectGroup { get; set; }

        public int ADetectGroupID { get; set; }

        public string MDetectGroup { get; set; }

        public int MDetectGroupID { get; set; }

        public Int64 TransId { get; set; }

        public string TransIdToString
        {
            get
            {
                return TransId.ToString();
            }
        }

        public int? RobotType { get; set; }

        public bool IsRobot { get; set; }

        public string ReasonList { get; set; }

        public string CustomerClassToString
        {
            get
            {
                if (this.CustomerClass != 0 && this.CustomerClass != -1)
                    return this.CustomerClass.ToString();
                else
                    return "-";
            }
        }

        public string PICVerified { get; set; }

        public int BetTypeID { get; set; }

        public bool IsPA { get; set; }

        public bool IsNormalLinkedPA { get; set; }

        public MatchMonitorQueryType TypeOfTicket { get; set; } = MatchMonitorQueryType.TicketFraud;
        #endregion

        #region function
        public bool FilterByHomeAway(string[] matchMonitorBetChoiceIds)
        {
            return matchMonitorBetChoiceIds.Any(x => string.Equals(BetTeam, x, StringComparison.OrdinalIgnoreCase));
        }

        public bool FilterBySite(string site)
        {
            return !string.IsNullOrWhiteSpace(site) ? site.Contains("A") || site.Contains(SubscriberType) : false;
        }

        public bool FilterByReasonAGroup(List<string> reasonList, IEnumerable<string> reasonGroupIDDic)
        {
            return
                (reasonList?.Any(y => (ReasonList ?? "-").Contains(y)) ?? true) &&
                (reasonGroupIDDic?.Contains(ADetectGroupID.ToString()) ?? true);
        }

        public bool FilterItem(MMDetailFilterViewModel filter, IEnumerable<string> reasonGroupIDDic, string site)
        {
            return
                (filter.CurrencyIdList?.Contains(CurrencyId) ?? true) &&
                (filter.HDPList?.Contains(BallHeadHDP()) ?? true) &&
                (filter.ScoreList?.Contains(Score ?? "-") ?? true) &&
                FilterByReasonAGroup(filter.ReasonList, reasonGroupIDDic) &&
                (filter.AmountList == null || filter.AmountList.Count() < 2 ? true : Amount >= filter.AmountList[0] && Amount <= filter.AmountList[1]) &&
                FilterBySite(site);
        }

        public MatchMonitorDetailItemViewModel MappingFraudTicketToAllTicket(Dictionary<long, MatchMonitorDetailItemViewModel> fraudMMTicketsDictionary)
        {
            if (fraudMMTicketsDictionary != null && fraudMMTicketsDictionary.TryGetValue(TransId, out var value))
            {
                return value;
            }

            return this;
        }

        public decimal BallHeadHDP()
        {
            if (BetTypeID == (int)MatchMonitorBetType.OU
                    || BetTypeID == (int)MatchMonitorBetType.FirsttOU
                    || BetTypeID == (int)MatchMonitorBetType.QuarterXOU)
            {
                return HDP;
            }

            return Math.Abs(HDP);
        }

        private void MappingTicketDetail(TicketDetailModel ticket)
        {
            this.CustomerClass = ticket.CustomerClass;
            //this.PICVerified = verifiedTransID,
            this.Danger1 = ticket.Danger1;
            this.Danger2 = ticket.Danger2;
            this.Danger3 = ticket.Danger3;
            this.Danger4 = ticket.Danger4;
            this.Danger5 = ticket.Danger5;
            this.Odds = ticket.Odds;
            this.BetID = ticket.TransId.ToString();
            this.BetTeam = ticket.BetTeam;
            this.CurrencyName = ticket.CurrencyName;
            this.CurrencyId = ticket.CurrencyId;
            this.Stake = ticket.Stake;
            this.Amount = ticket.OrgStake;
            this.HDP1 = ticket.Hdp1;
            this.HDP2 = ticket.Hdp2;
            this.Score = ticket.LiveIndicator ? (ticket.LiveHomeScore + " - " + ticket.LiveAwayScore) : "-";
            this.TicketStatus = ticket.TicketStatus;
            this.TransTime = ticket.TransDate;
            this.TransTimeToString = ticket.TransDate.ToString(Consts.FormatDateToString_ddMMhhmmss);
            this.MDetectGroup = "-";
            this.MDetectGroupID = -2;
            //this.ReasonList = "",         
            this.TransId = ticket.TransId;
            this.RobotType = null;
            this.IsRobot = false;
        }

        private void MappingCustInfo(CustInfoModel cust)
        {
            this.CTSCustID = cust.CTSCustID;
            this.CustID = cust.CustID;
            this.SubscriberType = cust.SubscriberType;
            this.Username = cust.UserName;
            this.IsPA = cust.ParentID == 50;
            this.IsNormalLinkedPA = (cust.ParentID == 200 && cust.TaggingType == 1);
        }
        #endregion
    }
}
