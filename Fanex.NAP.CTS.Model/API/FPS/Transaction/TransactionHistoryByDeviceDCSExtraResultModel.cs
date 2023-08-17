namespace Fanex.NAP.CTS.Model.API.FPS.Transaction
{
    using System.Collections.Generic;
    using System.Linq;

    public class TransactionHistoryByDeviceDCSExtraResultModel
    {
        public TransactionHistoryByDeviceDCSExtraResultModel()
        {
            this.Items = Enumerable.Empty<TransactionHistoryByDeviceDCSExtraItemModel>();
            this.TotalItem = 0;
        }

        public IEnumerable<TransactionHistoryByDeviceDCSExtraItemModel> Items { get; set; }

        public int TotalItem { get; set; }

        public bool IsTimeout { get; set; }
    }
}
