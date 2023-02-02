
namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using Fanex.NAP.CTS.Resource.Consts;
    using System;

    public class CoupleInfoViewModel
    {
        public long CoupleID { get; set; }

        public long CoupleInfoID { get; set; }

        public string CoupleInfoDetails { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedDateString { get { return CreatedDate.ToString(Consts.FormatDateToString_yyyyMMddHHmmss); } }

        public long CustID1 { get; set; }

        public long CustID2 { get; set; }

        public string Group
        {
            get
            {
                return $"Stake and IP Pattern detected on {CreatedDate.ToString("MMM dd, yyyy")}";
            }
        }
    }
}
