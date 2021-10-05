namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Model.TransactionReport;
    using Fanex.NAP.CTS.ViewModel.TransactionReport;
    using System.Collections.Generic;

    public interface ITransactionReportService
    {
        IEnumerable<TransactionModel> GetTransactions(TransactionFilterViewModel filterModel, out int totalItem);
    }
}
