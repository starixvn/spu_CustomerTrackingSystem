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
                var danger = string.Empty;
                var commaCount = 0;

                if (Danger1 != 0)
                {
                    danger += "OR" + Danger1;
                }                    

                if (Danger2 != 0)
                {                    
                    if (string.IsNullOrEmpty(danger))
                    {
                        danger += "A" + Danger2;
                    }
                    else
                    {
                        commaCount++;

                        danger += "," + AddSpaceDanger(commaCount) + "A" + Danger2;
                    }
                }
                    
                if (Danger3 != 0)
                {                   
                    if (string.IsNullOrEmpty(danger))
                    {
                        danger += "OS" + Danger3;
                    }
                    else
                    {
                        commaCount++;

                        danger += "," + AddSpaceDanger(commaCount) + "OS" + Danger3;
                    }
                }
                    
                if (Danger4 != 0)
                {                    
                    if (string.IsNullOrEmpty(danger))
                    {
                        danger += "B" + Danger4;
                    }
                    else
                    {
                        commaCount++;

                        danger += "," + AddSpaceDanger(commaCount) + "B" + Danger4;
                    }
                }
                    
                if (Danger5 != 0)
                {                    
                    if (string.IsNullOrEmpty(danger))
                    {
                        danger += "E" + Danger5;
                    }
                    else
                    {
                        commaCount++;

                        danger += "," + AddSpaceDanger(commaCount) + "E" + Danger5;
                    }
                }                    

                return (danger == string.Empty) ? "-" : danger;
            }
        }

        public decimal Odds { get; set; }

        public string BetID { get; set; }

        public string BetTeam { get; set; }

        public string BetFrom { get; set; }

        public string CurrencyName { get; set; }

        public short CurrencyId { get; set; }

        public decimal Stake { get; set; }

        public decimal Amount { get; set; }

        public decimal HDP1 { get; set; }

        public decimal HDP2 { get; set; }

        public string HDP
        {
            get
            {
                if (BetTypeID == (int)MatchMonitorBetType.OU
                    || BetTypeID == (int)MatchMonitorBetType.FirstOU
                    || BetTypeID == (int)MatchMonitorBetType.QuarterXOU)
                {
                    return HDP1.ToString();
                }

                if (!string.IsNullOrEmpty(BetTeam))
                {
                    if (string.Equals(BetTeam, "h", StringComparison.OrdinalIgnoreCase))
                    {
                        return (HDP2 - HDP1).ToString();
                    }
                    else if (string.Equals(BetTeam, "a", StringComparison.OrdinalIgnoreCase))
                    {
                        return (HDP1 - HDP2).ToString();
                    }
                }

                return "0";
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

        public bool IsLive { get; set; }

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
        
        public bool FilterByDanger(MMDetailFilterViewModel filter)
        {
            var isPass = (filter.Danger1List?.Contains(Danger1) ?? false) ||
                (filter.Danger2List?.Contains(Danger2) ?? false) ||
                (filter.Danger3List?.Contains(Danger3) ?? false) ||
                (filter.Danger4List?.Contains(Danger4) ?? false) ||
                (filter.Danger5List?.Contains(Danger5) ?? false);

            return isPass;
        }

        public bool FilterItem(MMDetailFilterViewModel filter, IEnumerable<string> reasonGroupIDDic, string site)
        {          
            var filterData =
                (filter.ScoreList?.Contains(Score ?? "-") ?? true) &&
                FilterBySite(site) &&
                FilterByDanger(filter) &&                
                (filter.CurrencyIdList?.Contains(CurrencyId) ?? true) &&
                FilterByReasonAGroup(filter.ReasonList, reasonGroupIDDic) &&      
               (filter.HDPBetTeamList?.Contains(BallHeadHDPOrBetTeam()) ?? true) &&
               (filter.AmountList == null || filter.AmountList.Count() < 2 ? true : Stake >= filter.AmountList[0] && Stake <= filter.AmountList[1]) &&
               (filter.TicketStatusList?.Contains(TicketStatus) ?? false);

            return filterData;
        }

        public MatchMonitorDetailItemViewModel MappingFraudTicketToAllTicket(Dictionary<long, MatchMonitorDetailItemViewModel> fraudMMTicketsDictionary)
        {
            if (fraudMMTicketsDictionary != null && fraudMMTicketsDictionary.TryGetValue(TransId, out var value))
            {
                return value;
            }

            return this;
        }

        public string BallHeadHDPOrBetTeam()
        {
            if (BetTypeID == (int)MatchMonitorBetType.TotalGoal
                || BetTypeID == (int)MatchMonitorBetType.FirstTotalGoal
                || BetTypeID == (int)MatchMonitorBetType.CorrectScore
                || BetTypeID == (int)MatchMonitorBetType.FirstCorrectScore)
            {
                return this.BetTeam;
            }

            if (BetTypeID == (int)MatchMonitorBetType.OU
                    || BetTypeID == (int)MatchMonitorBetType.FirstOU
                    || BetTypeID == (int)MatchMonitorBetType.QuarterXOU)
            {
                return HDP;
            }

            decimal.TryParse(this.HDP, out decimal hdpDecimal);

            if (hdpDecimal < 0)
            {
                return Math.Abs(hdpDecimal).ToString();
            }

            return HDP;
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
            this.BetTypeID = ticket.BetType;
            this.BetID = ticket.TransId.ToString();
            this.BetTeam = ticket.BetTeam;
            this.BetFrom = ticket.BetFrom;
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

        private string AddSpaceDanger(int commaCount)
        {
            return commaCount % 2 == 0 ? " " : string.Empty;
        }

        #endregion
    }
}
