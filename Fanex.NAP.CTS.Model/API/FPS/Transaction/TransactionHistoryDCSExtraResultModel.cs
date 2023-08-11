namespace Fanex.NAP.CTS.Model.API.FPS.Transaction
{
    using System.Collections.Generic;
    using System.Linq;

    public class TransactionHistoryDCSExtraResultModel
    {
        public TransactionHistoryDCSExtraResultModel()
        {
            this.Items = Enumerable.Empty<TransactionHistoryDCSExtraItemModel>();
            this.TotalItem = 0;
        }

        public IEnumerable<TransactionHistoryDCSExtraItemModel> Items { get; set; }

        public int TotalItem { get; set; }

        public bool IsTimeout { get; set; }
    }
}
