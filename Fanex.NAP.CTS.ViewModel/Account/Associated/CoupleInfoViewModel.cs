
namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using Fanex.NAP.CTS.Resource.Consts;
    using System;

    public class CoupleInfoViewModel
    {
        public int CoupleID { get; set; }

        public DateTime FromDate { get; set; }

        public string FromDateString { get { return FromDate.ToString(Consts.FormatDateToString_yyyyMMddHHmmss); } }

        public DateTime ToDate { get; set; }

        public string ToDateString { get { return ToDate.ToString(Consts.FormatDateToString_yyyyMMddHHmmss); } }

        public long CustID1 { get; set; }

        public long CustID2 { get; set; }

        public string Group
        {
            get
            {
                return $"Stake and IP Pattern from {FromDate.ToString("MMM dd")} to {ToDate.ToString("MMM dd")}";
            }
        }
    }
}
