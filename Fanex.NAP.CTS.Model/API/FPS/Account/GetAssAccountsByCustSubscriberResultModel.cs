namespace Fanex.NAP.CTS.Model.API.FPS.Account
{
    using System.Collections.Generic;
    using System.Linq;

    public class GetAssAccountsByCustSubscriberResultModel
    {
        public GetAssAccountsByCustSubscriberResultModel()
        {
            this.Items = Enumerable.Empty<GetAssAccountsByCustSubscriberItemModel>();
            this.TotalItem = 0;
        }

        public IEnumerable<GetAssAccountsByCustSubscriberItemModel> Items { get; set; }

        public int TotalItem { get; set; }
    }
}
