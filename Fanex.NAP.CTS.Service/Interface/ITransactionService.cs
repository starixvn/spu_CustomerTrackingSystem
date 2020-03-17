namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.Transaction;
    using Fanex.NAP.CTS.ViewModel.Account.TransactionHistory;
    using System;
    using System.Collections.Generic;

    public interface ITransactionService
    {
        IEnumerable<HistoryTransactionModel> GetTransactionHistory(long ctsCustId, int subscriberId, DateTime fromDate, DateTime toDate, int pageSize, int pageNumber, out int totalItem);

        IPInformation GetIPInformation(string ipAddress);
    }
}