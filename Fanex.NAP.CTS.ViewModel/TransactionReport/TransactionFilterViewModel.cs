namespace Fanex.NAP.CTS.ViewModel.TransactionReport
{
    using System;

    public class TransactionFilterViewModel
    {
        public int draw { get; set; }

        public int start { get; set; }

        public int length { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }            

        public string SubscriberIDs { get; set; }

    }
}
