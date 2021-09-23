namespace Fanex.NAP.CTS.Model.TransactionReport
{
    public class TransactionModel
    {
        public int SubscriberId { get; set; }

        public string SubscriberName { get; set; }

        public int TotalTrans { get; set; }

        public int ValidTrans { get; set; }

        public int InvalidTrans { get; set; }

        public int BrowserlessValidTrans { get; set; }
    }
}
