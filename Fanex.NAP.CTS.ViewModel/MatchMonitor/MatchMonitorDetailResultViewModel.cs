namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using System;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class MatchMonitorDetailResultViewModel
    {
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

        public decimal Odds { get; set; }

        public int AssociationIrrigation { get; set; }

        public Int64 BetID { get; set; }

        public string Currency { get; set; }

        public decimal Amount { get; set; }

        public decimal HDP1 { get; set; }

        public decimal HDP2 { get; set; }

        public string Score { get; set; }

        public string TransTimeToString { get; set; }

        public DateTime TransTime { get; set; }

        public string Group { get; set; }

        public string BetTeam { get; set; }

        public int TicketStatus { get; set; }

        public string PICVerified { get; set; }

        public int? RobotType { get; set; }

        public bool IsRobot { get; set; }

        public Int64 TransId { get; set; }

        public int BetTypeID { get; set; }

        public string TransIdToString
        {
            get
            {
                return TransId.ToString();
            }
        }

        public string BetChoice
        {
            get
            {
                if (string.IsNullOrEmpty(BetTeam))
                {
                    return BetTeam;
                }

                if (BetTypeID == (int)MatchMonitorBetType.FirsttOU || BetTypeID == (int)MatchMonitorBetType.OU || BetTypeID == (int)MatchMonitorBetType.QuarterXOU)
                {
                    if (string.Equals(BetTeam, "h", StringComparison.OrdinalIgnoreCase))
                    {
                        return nameof(MatchMonitorBetChoiceId.O);
                    }
                    else if (string.Equals(BetTeam, "a", StringComparison.OrdinalIgnoreCase))
                    {
                        return nameof(MatchMonitorBetChoiceId.U);
                    }
                }

                return BetTeam.ToUpper();
            }
        }

        public decimal HDP
        {
            get
            {
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
                    else
                    {
                        return HDP1;
                    }
                }

                return 0;
            }
        }

        public string DangerToString
        {
            get
            {
                string danger = string.Empty;

                if (Danger1 != 0)
                {
                    danger += "Ori" + Danger1;
                }

                if (Danger2 != 0)
                {
                    danger += string.IsNullOrEmpty(danger) ? "ABI" + Danger2 : "-ABI" + Danger2;
                }

                if (Danger3 != 0)
                {
                    danger += string.IsNullOrEmpty(danger) ? "OS" + Danger3 : "-OS" + Danger3;
                }

                if (Danger4 != 0)
                {
                    danger += string.IsNullOrEmpty(danger) ? "BkB" + Danger4 : "-BkB" + Danger4;
                }

                if (Danger5 != 0)
                {
                    danger += string.IsNullOrEmpty(danger) ? "ES" + Danger5 : "-ES" + Danger5;
                }

                return (danger == string.Empty) ? "-" : danger;
            }
        }

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
    }
}