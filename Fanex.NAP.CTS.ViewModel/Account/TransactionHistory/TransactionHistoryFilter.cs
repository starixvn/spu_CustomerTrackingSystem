namespace Fanex.NAP.CTS.ViewModel.Account.TransactionHistory
{
    using Fanex.NAP.CTS.Model;

    public class TransactionHistoryFilter
    {
        public int AccountId { get; set; }    

        public TransactionHistoryLength ReportLength { get; set; }
    }
}