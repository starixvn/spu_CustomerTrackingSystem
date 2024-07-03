namespace Fanex.NAP.CTS.ViewModel.Account
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Resource.Consts;

    public class CustomerClassificationHistoryViewModel
    {
        public CustomerClassificationHistoryViewModel(CustomerClassificationHistoryModel data)
        {
            this.CreatedTimeString = data.CreatedTime.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss);
            this.SourceTypeName = data.SourceTypeName;
            this.CategoryName = (data.SourceTypeID == (int)SourceTypeID.NoShow_Detail) ? "-" : (data.CategoryName != null ? data.CategoryName : string.Empty);
            this.CustomerClass = data.CustomerClass;
            this.Reason = data.Remark;
            //Reason = GetReason(x, roleID);
            //this.TurnOver = data.TurnOver;
            //this.Winloss = data.WinLoss;
            //this.Margin = data.Margin;
            //this.BetCount = data.BetCount;
            //this.ActiveDays = data.ActiveDays;           
            this.CreatedBy = data.CreatedBy;
        }

        public string CreatedTimeString { get; set; }

        public string SourceTypeName { get; set; }

        public string CategoryName { get; set; }

        public string CustomerClass { get; set; }
        
        public int CategoryID { get; set; }

        public string Reason { get; set; }

        public decimal TurnOver { get; set; }

        public decimal Winloss { get; set; }

        public decimal Margin { get; set; }

        public decimal BetCount { get; set; }

        public int ActiveDays { get; set; }      

        public string CreatedBy { get; set; }

        public string TaggingType { get; set; }

        public bool IsParlay { get; set; }

        public string SportName { get; set; }

        public int IssueTypeID { get; set; }
         
    }
}
