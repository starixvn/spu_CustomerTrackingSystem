namespace Fanex.NAP.CTS.Model.API.FPS.Transaction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TransactionParamModel
    {
        public int SubscriberID { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public long? CTSCustID { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

    }
}
