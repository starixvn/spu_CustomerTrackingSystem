namespace Fanex.NAP.CTS.ViewModel.Account.CustomerCategory
{
    public class SaveMarkCustomerCategoryViewModel
    {
        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public int RoleID { get; set; }

        public int SubscriberID { get; set; }

        public bool IsLicensee { get; set; }

        public bool IsVVip { get; set; }

        public int CategoryID { get; set; }           

        public bool IsAffectAllDownlines { get; set; }

        public string Remark { get; set; }

    }
}
