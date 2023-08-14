namespace Fanex.NAP.CTS.Model.API.FPS.Transaction
{
    using System;

    public class TransactionHistoryByAccountParamModel
    {
        public int SubscriberID { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public long? CTSCustID { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}