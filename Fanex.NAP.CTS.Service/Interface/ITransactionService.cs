namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.Transaction;
    using Fanex.NAP.CTS.ViewModel.Transaction;
    using System.Collections.Generic;

    public interface ITransactionService
    {
        IEnumerable<Transaction> GetTransactionHistory(TransactionHistoryFilter filter);

        IPInformation GetIPInformation(string ipAddress);
    }
}