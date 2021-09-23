namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.NAP.CTS.Model.TransactionReport;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.TransactionReport;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TransactionReportService : ITransactionReportService
    {
        public IEnumerable<TransactionModel> GetTransactions(TransactionFilterViewModel filterModel)
        {
            return GetTransactions_DataTest();
        }

        private IEnumerable<TransactionModel> GetTransactions_DataTest()
        {
            var transactionList = new List<TransactionModel>();

            for (int i = 0; i < 10; i++)
            {
                transactionList.Add(new TransactionModel
                {
                    SubscriberId = 1,
                    SubscriberName = "12Bet",
                    ValidTrans = i + 1000000,
                    InvalidTrans = i + 2000000,
                    BrowserlessValidTrans = i + 3000000,
                    TotalTrans = i + 6000000
                });
            }

            return transactionList;
        }
    }
}
