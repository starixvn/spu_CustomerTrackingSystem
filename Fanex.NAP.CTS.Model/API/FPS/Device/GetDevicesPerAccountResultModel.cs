using System.Collections.Generic;

namespace Fanex.NAP.CTS.Model.API.FPS.Device
{
    public class GetDevicesPerAccountResultModel
    {
        public IEnumerable<GetDevicesPerAccountItemModel> Items { get; set; }

        public int TotalRows { get; set; }
    }
}
