namespace Fanex.NAP.CTS.Model.Account.AssociatedAccount
{
    using Fanex.NAP.CTS.Resource.Consts;
    using System;
    using System.Globalization;

    public class CoupleInfoModel
    {
        public long CoupleID { get; set; }

        public string CoupleInfoDetails { get; set; }

        public DateTime FromDate
        {
            get
            {
                if (!string.IsNullOrEmpty(FromDateString))
                {
                    DateTime.TryParseExact(FromDateString, Consts.FormatDateToString_yyyyMMddHHmmss, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime value);

                    return value;
                }

                return DateTime.MinValue;
            }
        }

        public string FromDateString { get; set; }

        public DateTime ToDate
        {
            get
            {
                if (!string.IsNullOrEmpty(ToDateString))
                {
                    DateTime.TryParseExact(ToDateString, Consts.FormatDateToString_yyyyMMddHHmmss, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime value);

                    return value;
                }

                return DateTime.MinValue;
            }
        }

        public string ToDateString { get; set; }

        public long CustID1 { get; set; }

        public long CustID2 { get; set; }
    }
}
