namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using System;
   
    [Serializable]
    public class ExportExcelViewModel
    {
        public Int64 CTSCustID { get; set; }

        public Int64 CustID { get; set; }

        public string SubscriberType { get; set; }

        public string Username { get; set; }

        public int CustomerClass { get; set; }

        public bool IsRobot { get; set; }

        public string DangerToString { get; set; }

        public decimal Odds { get; set; }

        public string BetID { get; set; }

        public string BetTeam { get; set; }

        public string Currency { get; set; }

        public decimal Amount { get; set; }    

        public decimal HDP { get; set; }

        public string Score { get; set; }
   
        public string TransTimeToString { get; set; }

        public string Group { get; set; }
    
        public string ReasonList { get; set; }

        public string PICVerified { get; set; }
    }
}
