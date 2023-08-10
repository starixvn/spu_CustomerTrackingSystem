namespace Fanex.NAP.CTS.Model.API.FPS.Transaction
{
    using System.Collections.Generic;
    using System.Linq;

    public class TransactionHistoryDcsResultModel
    {
        public TransactionHistoryDcsResultModel()
        {
            this.Items = Enumerable.Empty<TransactionHistoryDcsItemModel>();
            this.TotalItem = 0;
        }

        public IEnumerable<TransactionHistoryDcsItemModel> Items { get; set; }

        public int TotalItem { get; set; }

        public bool IsTimeout { get; set; }
    }
}
