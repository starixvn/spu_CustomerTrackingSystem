using System.Collections.Generic;

namespace Fanex.NAP.CTS.Model.API.FPS.Account
{
    public class GetAccountByDeviceResultModel
    {
        public IEnumerable<GetAccountByDeviceItemModel> Items { get; set; }
    }
}
