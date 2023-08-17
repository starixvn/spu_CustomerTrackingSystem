namespace Fanex.NAP.CTS.Model.API.FPS.Transaction
{
    using System;

    public class TransactionHistoryByDeviceParamModel
    {
        public int SubscriberID { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public long DeviceID { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}
