using System.Collections.Generic;

namespace Fanex.NAP.CTS.Model.API.FPS.Device
{
    public class GetDeviceByAccountResultModel
    {
        public IEnumerable<GetDeviceByAccountItemModel> Items { get; set; }
    }
}
