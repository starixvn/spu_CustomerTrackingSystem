namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using System;

    public class MatchMonitorDetailResultViewModel
    {
        public Int64 CTSCustID { get; set; }

        public Int64 CustID { get; set; }

        public string SubscriberType { get; set; }

        public string Username { get; set; }

        public int Danger1 { get; set; }

        public int Danger2 { get; set; }

        public int Danger3 { get; set; }

        public decimal Odds { get; set; }

        public Int64 BetID { get; set; }

        public string Currency { get; set; }

        public bool LiveIndicator { get; set; }

        public decimal Amount { get; set; }

        public decimal HDP { get; set; }

        public string Score { get; set; }

        public string TransTime { get; set; }

        public string ReasonGroup { get; set; }

        public string BetTeam { get; set; }

        public bool IsHomeSide
        {
            get
            {
                if (!string.IsNullOrEmpty(BetTeam))
                {
                    return string.Equals(BetTeam, "a", StringComparison.OrdinalIgnoreCase) ? true : false;
                }

                return true;
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
                    danger += string.IsNullOrEmpty(danger) ? "ABI" : " - ABI" + Danger2;
                }
                
                if (Danger3 != 0)
                {
                    danger += string.IsNullOrEmpty(danger) ? "OS" : " - OS"  + Danger3;
                }

                return (danger == string.Empty) ? "-" : danger;
            }
        }
    }
}