namespace Fanex.NAP.CTS.ViewModel.Account.CustomerCategory
{  
    public class SaveUnmarkCustomerCategoryViewModel
    {
        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public int RoleID { get; set; }

        public int SubscriberID { get; set; }

        public bool IsExceptDirectDownline { get; set; }
    }
}
