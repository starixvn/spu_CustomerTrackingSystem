namespace Fanex.NAP.CTS.Model.Account.AssociatedAccount
{
    using Fanex.NAP.CTS.Resource.Consts;
    using System;
    using System.Globalization;

    public class CoupleInfoModel
    {
        public long CoupleID { get; set; }

        public string CoupleInfoDetails { get; set; }

        public DateTime CreatedDate
        {
            get
            {
                if (!string.IsNullOrEmpty(CreatedDateString))
                {
                    DateTime.TryParseExact(CreatedDateString, Consts.FormatDateToString_yyyyMMddHHmmss, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime value);

                    return value;
                }

                return DateTime.MinValue;
            }
        }

        public string CreatedDateString { get; set; }

        public long CustID1 { get; set; }

        public long CustID2 { get; set; }
    }
}
