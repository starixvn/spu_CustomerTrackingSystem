namespace Fanex.NAP.CTS.ViewModel.TransactionReport
{   
    public class TransactionViewModel
    {
        public int SubscriberId { get; set; }

        public string SubscriberName { get; set; }

        public int TotalTrans { get; set; }

        public int ValidTrans { get; set; }

        public int InvalidTrans { get; set; }

        public int BrowserlessValidTrans { get; set; }
    }
}
