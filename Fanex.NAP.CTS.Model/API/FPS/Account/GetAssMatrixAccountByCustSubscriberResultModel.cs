namespace Fanex.NAP.CTS.Model.API.FPS.Account
{
    using System.Collections.Generic;
    using System.Linq;

    public class GetAssMatrixAccountByCustSubscriberResultModel
    {
        public GetAssMatrixAccountByCustSubscriberResultModel()
        {
            this.Items = Enumerable.Empty<GetAssMatrixAccountByCustSubscriberItemModel>();
        }

        public IEnumerable<GetAssMatrixAccountByCustSubscriberItemModel> Items { get; set; }
    }
}
