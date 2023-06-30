namespace Fanex.NAP.CTS.Model.API.FPS.Transaction
{
    using System.Collections.Generic;
    using System.Linq;

    public class LastestTransactionResultModel
    {
        public LastestTransactionResultModel()
        {
            this.Items = Enumerable.Empty<LastestTransactionItemModel>();
            this.TotalItem = 0;
        }

        public IEnumerable<LastestTransactionItemModel> Items { get; set; }

        public int TotalItem { get; set; }
    }
}
