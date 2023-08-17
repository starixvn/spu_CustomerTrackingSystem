namespace Fanex.NAP.CTS.Model.API.FPS.Transaction
{
    using System.Collections.Generic;
    using System.Linq;

    public class LatestTransactionResultModel
    {
        public LatestTransactionResultModel()
        {
            this.Items = Enumerable.Empty<LatestTransactionItemModel>();
            this.TotalItem = 0;
        }

        public IEnumerable<LatestTransactionItemModel> Items { get; set; }

        public int TotalItem { get; set; }
    }
}
