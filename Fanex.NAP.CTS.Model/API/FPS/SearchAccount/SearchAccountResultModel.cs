namespace Fanex.NAP.CTS.Model.API.FPS.SearchAccount
{
    using System.Collections.Generic;
    using System.Linq;

    public class SearchAccountResultModel
    {
        public SearchAccountResultModel()
        {
            this.Items = Enumerable.Empty<SearchAccountItemModel>();
            this.TotalItem = 0;
        }

        public IEnumerable<SearchAccountItemModel> Items { get; set; }

        public int TotalItem { get; set; }
    }
}
