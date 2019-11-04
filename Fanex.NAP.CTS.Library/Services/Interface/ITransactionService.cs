namespace Fanex.NAP.CTS.Library.Services
{
    using Fanex.NAP.CTS.Library.Models.Transaction;
    using Fanex.NAP.CTS.Library.ViewModels.Transaction;
    using System.Collections.Generic;

    public interface ITransactionService
    {
        IEnumerable<Transaction> GetTransactionHistory(TransactionHistoryFilter filter);

        IPInformation GetIPInformation(string ipAddress);
    }
}