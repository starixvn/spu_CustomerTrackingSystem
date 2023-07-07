namespace Fanex.NAP.CTS.Model.API.FPS.Account
{
    using System.Collections.Generic;
    using System.Linq;

    public class GetAssMatrixAccountByCustSubscriberResultModel
    {
        public GetAssMatrixAccountByCustSubscriberResultModel()
        {
            this.Items = Enumerable.Empty<GetAssMatrixAccountByCustSubscriberItemModel>();
            this.TotalPages = 0;
            this.TotalRows = 0;
        }

        public IEnumerable<GetAssMatrixAccountByCustSubscriberItemModel> Items { get; set; }

        public int TotalPages { get; set; }

        public int TotalRows { get; set; }
    }
}
