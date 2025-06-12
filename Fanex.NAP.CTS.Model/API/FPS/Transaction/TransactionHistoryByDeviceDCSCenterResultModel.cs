
namespace Fanex.NAP.CTS.Model.API.FPS.Transaction
{
    using System.Collections.Generic;
    using System.Linq;
    public class TransactionHistoryByDeviceDCSCenterResultModel
    {
        public TransactionHistoryByDeviceDCSCenterResultModel()
        {
            this.Items = Enumerable.Empty<TransactionHistoryByDeviceDCSCenterItemModel>();
            this.TotalItem = 0;
        }

        public IEnumerable<TransactionHistoryByDeviceDCSCenterItemModel> Items { get; set; }

        public int TotalItem { get; set; }

        public bool IsTimeout { get; set; }
    }
}
