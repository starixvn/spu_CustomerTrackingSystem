
namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using Fanex.NAP.CTS.Resource.Consts;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CoupleInfoViewModel
    {
        public CoupleInfoViewModel()
        {
            CoupleInfo = Enumerable.Empty<CoupleInfoModel>();
        }

        public IEnumerable<CoupleInfoModel> CoupleInfo { get; set; }

        public DateTime FirstCreatedDate { get; set; }

        public string CoupleInfoDetails { get { return JsonConvert.SerializeObject(CoupleInfo); } }
    }
}