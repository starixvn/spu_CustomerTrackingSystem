namespace Fanex.NAP.CTS.Model.LatestTransaction
{
    using System;

    public class LatestTransactionModel
    {
        public DateTime TransactionDate { get; set; }

        public string Username { get; set; }

        public string LoginID { get; set; }

        public string Device { get; set; }

        public string IP { get; set; }

        public string RobotTracking { get; set; }

        public string Action { get; set; }

        public string ActionResult { get; set; }

        public string DeviceType { get; set; }

        public string Browser { get; set; }

        public string Domain { get; set; }
    }
}
