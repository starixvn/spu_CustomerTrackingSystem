using System.Collections.Generic;

namespace Fanex.NAP.CTS.Model.API.FPS.Account
{
    public class GetAccountsPerDeviceResultModel
    {
        public IEnumerable<GetAccountsPerDeviceItemModel> Items { get; set; }

        public int TotalRows { get; set; }
    }
}
