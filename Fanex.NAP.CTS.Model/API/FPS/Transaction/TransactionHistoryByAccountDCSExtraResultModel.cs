namespace Fanex.NAP.CTS.Model.API.FPS.Transaction
{
    using System.Collections.Generic;
    using System.Linq;

    public class TransactionHistoryByAccountDCSExtraResultModel
    {
        public TransactionHistoryByAccountDCSExtraResultModel()
        {
            this.Items = Enumerable.Empty<TransactionHistoryByAccountDCSExtraItemModel>();
            this.TotalItem = 0;
        }

        public IEnumerable<TransactionHistoryByAccountDCSExtraItemModel> Items { get; set; }

        public int TotalItem { get; set; }

        public bool IsTimeout { get; set; }
    }
}