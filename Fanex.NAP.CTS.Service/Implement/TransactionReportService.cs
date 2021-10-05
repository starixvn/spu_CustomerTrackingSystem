namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Criteria.TransactionReport;
    using Fanex.NAP.CTS.Model.TransactionReport;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.TransactionReport;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;    

    public class TransactionReportService : ITransactionReportService
    {
        public IEnumerable<TransactionModel> GetTransactions(TransactionFilterViewModel filterModel, out int totalItem)
        {
            var criteria = new TransactionReportCriteria
            {
                SubscriberIDList = filterModel.SubscriberIDs,
                FromDate = filterModel.FromDate,
                ToDate = filterModel.ToDate,
                Take = filterModel.length,
                Skip = filterModel.start,
                TotalItem = 0
            };

            var result = DataService.GetData<TransactionModel>(criteria);

            totalItem = criteria.TotalItem;

            return result;
        }        
    }
}
