namespace Fanex.NAP.CTS.Model.API.FPS.Device
{
    using System.Collections.Generic;
    using System.Linq;

    public class GetAssDevicesByCustSubscriberResultModel
    {
        public GetAssDevicesByCustSubscriberResultModel()
        {
            this.Items = Enumerable.Empty<GetAssDevicesByCustSubscriberItemModel>();
        }

        public IEnumerable<GetAssDevicesByCustSubscriberItemModel> Items { get; set; }
    }
}
