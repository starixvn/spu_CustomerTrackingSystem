namespace Fanex.NAP.CTS.ViewModel.Account.CustomerCategoryBySport
{
    using Fanex.NAP.CTS.Model.Account.CustomerCategoryBySport;
    using Fanex.NAP.CTS.Resource.Consts;
    using System;

    public class GetCustomerCategoryHistoryBySportViewModel
    {
        public GetCustomerCategoryHistoryBySportViewModel(GetCustomerCategoryHistoryBySportModel data)
        {
            this.ID = data.ID;
            this.CustID = data.CustID;
            this.CTSCustID = data.CTSCustID;
            this.SportID = data.SportID;
            this.CategoryID = data.CategoryID;
            this.CategoryName = data.CategoryName;
            this.TurnoverRM = data.TurnoverRM;
            this.WinlossRM = data.WinlossRM;
            this.BetCount = data.BetCount;
            this.ActiveDays = data.ActiveDays;            
            this.CustomerClass = (data.TargetCC == -1 || data.TargetCC == 0) ? "-" : data.TargetCC.ToString();
            this.CreatedTimeString = data.LastModifiedDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss);
            this.Reason = "TEST";
            this.CreatedBy = data.CreatedBy;
        }

        public long ID { get; set; }

        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public int SportID { get; set; }

        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public decimal TurnoverRM { get; set; }

        public decimal WinlossRM { get; set; }

        public long BetCount { get; set; }

        public int ActiveDays { get; set; }

        public string CustomerClass { get; set; }

        public string CreatedTimeString { get; set; }

        public string Reason { get; set; }

        public string CreatedBy { get; set; }
    }
}
