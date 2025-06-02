namespace Fanex.NAP.CTS.Model.API.FPS.SearchAccount
{
    using Fanex.NAP.CTS.Model.API.FPS.Search;
    using System.Collections.Generic;
    using System.Linq;

    public class SearchDeviceResultModel
    {
        public SearchDeviceResultModel()
        {
            this.Items = Enumerable.Empty<SearchDeviceItemModel>();
        }

        public IEnumerable<SearchDeviceItemModel> Items { get; set; }

    }
}
