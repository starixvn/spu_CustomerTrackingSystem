namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.LatestTransaction;
    using Fanex.NAP.CTS.Model.Transaction;
    using System.Collections.Generic;

    public interface ITransactionService
    {
        IEnumerable<HistoryTransactionModel> GetTransactionHistory(long ctsCustId, int dateRange, int pageSize, int pageNumber, out int totalItem);

        IEnumerable<LatestTransactionModel> GetLatestTransaction(int subscriberId, int transactionLength);
    }
}