namespace Fanex.NAP.CTS.Model.API.FPS.Account
{
    using System.Collections.Generic;
    using System.Linq;

    public class GetAssAccountsByCustSubscriberResultModel
    {
        public GetAssAccountsByCustSubscriberResultModel()
        {
            this.Items = Enumerable.Empty<GetAssAccountsByCustSubscriberItemModel>();
        }

        public IEnumerable<GetAssAccountsByCustSubscriberItemModel> Items { get; set; }
    }
}
