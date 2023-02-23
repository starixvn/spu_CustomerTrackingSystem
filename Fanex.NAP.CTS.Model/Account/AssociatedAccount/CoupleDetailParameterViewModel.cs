namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using Fanex.NAP.CTS.Resource.Consts;
    using System;
    using System.Collections.Generic;

    public class CoupleDetailParameterViewModel 
    {
        public CoupleDetailParameterViewModel()
        {
            CoupleInfo = new List<CoupleDetailParameterModel>();
        }

        public List<CoupleDetailParameterModel> CoupleInfo { get; set; }

        public DateTime FirstCreatedDate { get; set; }

        public string FirstCreatedDateString { get { return FirstCreatedDate.ToString(Consts.FormatDateToString_MMMddyyyy); } }
    }
}
