namespace Fanex.NAP.CTS.Library.ViewModels.Transaction
{
    using Fanex.NAP.CTS.Library.Models;

    public class TransactionHistoryFilter
    {
        public int AccountId { get; set; }    

        public TransactionHistoryLength ReportLength { get; set; }
    }
}