namespace Fanex.NAP.CTS.ViewModel.TransactionReport
{   
    public class TransactionViewModel
    {
        public string Subscriber { get; set; }

        public int TotalTrans { get; set; }

        public int TotalValidTrans { get; set; }

        public int ValidTransNewDevice { get; set; }

        public int ValidTransOldDevice { get; set; }

        public int ValidTransRecoverDevice { get; set; }

        public int ValidTransNoDevice { get; set; }

        public int InvalidTrans { get; set; }

        public int Browserless { get; set; }
    }
}
