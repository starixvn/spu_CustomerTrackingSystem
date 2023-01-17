
namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using Fanex.NAP.CTS.Resource.Consts;
    using System;
    using System.Globalization;

    public class CoupleInfoViewModel
    {
        public int CoupleID { get; set; }
        public DateTime FromDate { get {
                if (!string.IsNullOrEmpty(FromDateString))
                {
                    DateTime.TryParseExact(FromDateString, Consts.FormatDateToString_yyyyMMddHHmmss, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime eventDate);

                    return eventDate;
                }

                return DateTime.MinValue;
            } }
        public string FromDateString { get; set; }
        public DateTime ToDate
        {
            get
            {
                if (!string.IsNullOrEmpty(ToDateString))
                {
                    DateTime.TryParseExact(ToDateString, Consts.FormatDateToString_yyyyMMddHHmmss, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime eventDate);

                    return eventDate;
                }

                return DateTime.MinValue;
            }
        }
        public string ToDateString { get; set; }

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
