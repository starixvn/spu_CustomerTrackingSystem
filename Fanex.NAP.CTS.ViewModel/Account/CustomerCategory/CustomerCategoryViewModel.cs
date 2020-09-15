namespace Fanex.NAP.CTS.ViewModel.Account.CustomerCategory
{    
    public class CustomerCategoryViewModel
    {
        public int ParentCategoryId { get; set; }

        public string ParentCategoryName { get; set; }

        public int CategoryType { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string SportGroupIdList { get; set; }

        public string SportGroupNameList { get; set; }

        public string Remark { get; set; }

        public string LastModifiedBy { get; set; }

        public string LastModifiedDate { get; set; }

        public string NextScanDatePeriod { get; set; }

        public bool IsEnabledDelete { get; set; }
    }
  
}