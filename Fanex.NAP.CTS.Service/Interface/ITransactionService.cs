namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.Transaction;
    using Fanex.NAP.CTS.ViewModel.Account.TransactionHistory;
    using System;
    using System.Collections.Generic;

    public interface ITransactionService
    {
        IEnumerable<HistoryTransactionModel> GetTransactionHistory(long ctsCustId, int subscriberId, int dateRange, int pageSize, int pageNumber, out int totalItem);
    }
}