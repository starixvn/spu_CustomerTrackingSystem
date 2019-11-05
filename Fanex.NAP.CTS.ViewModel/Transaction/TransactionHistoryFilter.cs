namespace Fanex.NAP.CTS.ViewModel.Transaction
{
    using Fanex.NAP.CTS.Model;

    public class TransactionHistoryFilter
    {
        public int AccountId { get; set; }    

        public TransactionHistoryLength ReportLength { get; set; }
    }
}