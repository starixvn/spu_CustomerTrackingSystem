namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.NAP.CTS.Core.Service;
    using Fanex.NAP.CTS.Criteria.TransactionReport;
    using Fanex.NAP.CTS.Model.TransactionReport;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.TransactionReport;
    using System.Collections.Generic;

    public class TransactionReportService : ITransactionReportService
    {
        private readonly ICriteriaService _repository;

        public TransactionReportService()
        {
            _repository = new CriteriaService();
        }

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

            var result = _repository.Fetch<TransactionModel>(criteria);

            totalItem = criteria.TotalItem;

            return result;
        }        
    }
}
