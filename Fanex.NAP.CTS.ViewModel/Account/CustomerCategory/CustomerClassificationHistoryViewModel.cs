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

            this.Reason = GetReason(data);
        }

        public string CreatedTimeString { get; set; }

        public string SourceTypeName { get; set; }

        public string CategoryName { get; set; }

        public string CustomerClass { get; set; }
            
        public string Reason { get; set; }
      
        public string CreatedBy { get; set; }

        private string GetReason(CustomerClassificationHistoryModel data)
        {
            var reason = string.Empty;

            if (!string.IsNullOrEmpty(data.Remark))
            {
                if (data.IsGetSportName)
                {
                    reason = data.Remark.Replace("[SportName]", data.SportName);
                }              
                else
                {
                    reason = data.Remark.Replace("[SportName]", "Soccer/Basketball");
                }
            }          

            return reason;
        }
              
    }
}
