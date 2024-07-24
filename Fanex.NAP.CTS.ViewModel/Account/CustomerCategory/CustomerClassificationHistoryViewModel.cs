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
            this.CreatedBy = data.CreatedBy;
          
            if (data.IsGetSportName && !string.IsNullOrEmpty(data.Remark))
            {
                this.Reason = data.Remark.Replace("[SportName]", data.SportName);
            }
            else
            {
                this.Reason = data.Remark;
            }            
        }

        public string CreatedTimeString { get; set; }

        public string SourceTypeName { get; set; }

        public string CategoryName { get; set; }

        public string CustomerClass { get; set; }
            
        public string Reason { get; set; }
      
        public string CreatedBy { get; set; }
              
    }
}
