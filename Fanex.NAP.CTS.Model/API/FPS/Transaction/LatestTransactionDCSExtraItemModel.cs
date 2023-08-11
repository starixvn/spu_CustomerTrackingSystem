using System;

namespace Fanex.NAP.CTS.Model.API.FPS.Transaction
{
    public class LatestTransactionDCSExtraItemModel
    {
        public DateTime TransTime { get; set; }

        public long TransID { get; set; }

        public long AccountID { get; set; }

        public int SubscriberID { get; set; }

        public string LoginName { get; set; }

        public string FirstDeviceCode { get; set; }

        public string Action { get; set; }

        public string ActionResult { get; set; }

        public string OS { get; set; }

        public string Browser { get; set; }

        public string URLDetails { get; set; }

        public string IP { get; set; }

        public string RobotTracking { get; set; }

        public string Country { get; set; }

        public string Region { get; set; }

        public string City { get; set; }

        public string ISP { get; set; }
    }
}
