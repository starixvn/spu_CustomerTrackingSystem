namespace Fanex.NAP.CTS.Model.Account
{
    using System;    

    public class RobotClassification
    {
        public long CustID { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int RobotType { get; set; }

        public decimal Turnover { get; set; }

        public decimal Winloss { get; set; }       

        public decimal BetCount { get; set; }

        public int ActiveDays { get; set; }        
        
    }
}
