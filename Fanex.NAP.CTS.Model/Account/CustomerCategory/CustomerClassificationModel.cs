namespace Fanex.NAP.CTS.Model.Account
{
    using Fanex.NAP.CTS.Resource.Consts;
    using System;

    public class CustomerClassificationModel
    {
        public int ParentCategoryID { get; set; }

        public string ParentCategoryName { get; set; }

        public int CategoryType { get; set; }

        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public string SportGroupIDList { get; set; }

        public string SportGroupNameList { get; set; }

        public string Remark { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }        

        public bool IsEnabledDelete { get; set; }

        public DateTime NextScanFromDate { get; set; }

        public DateTime NextScanToDate { get; set; }

        public string NextScanDatePeriod
        {
            get
            {
                if (NextScanFromDate == DateTime.MinValue || NextScanToDate == DateTime.MinValue)
                {
                    return null;
                }

                if (Equals(NextScanFromDate, NextScanToDate))
                {
                    return NextScanFromDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss);
                }

                return NextScanFromDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss) + " - " + NextScanToDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss);
            }
        }
    }
}
