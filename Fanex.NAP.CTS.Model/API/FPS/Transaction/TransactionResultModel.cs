namespace Fanex.NAP.CTS.Model.API.FPS.Transaction
{
    using System.Collections.Generic;
    using System.Linq;

    public class TransactionResultModel
    {
        public TransactionResultModel()
        {
            this.Items = Enumerable.Empty<TransactionItemModel>();
            this.TotalItem = 0;
        }

        public IEnumerable<TransactionItemModel> Items { get; set; }

        public int TotalItem { get; set; }
    }
}
