namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using System;

    public class CustInfoModel
    {
        public Int64 CTSCustID { get; set; }

        public Int64 CustID { get; set; }

        public string UserName { get; set; }

        public int SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public string Site { get; set; }

        public string Currency { get; set; }

        public string SubscriberType { get; set; }

        public bool IsRobotUser { get; set; }

        ///TODO
        public int RobotBetting { get; set; }
    }
}