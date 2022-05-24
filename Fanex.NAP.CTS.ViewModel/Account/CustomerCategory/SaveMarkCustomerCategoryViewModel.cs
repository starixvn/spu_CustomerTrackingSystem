namespace Fanex.NAP.CTS.ViewModel.Account.CustomerCategory
{
    public class SaveMarkCustomerCategoryViewModel
    {
        public long CTSCustID { get; set; }

        public bool IsVVip { get; set; }   

        public string CategoryIDs { get; set; }

        public bool IsAffectAllDownlines { get; set; }

        public string Comment { get; set; }

    }
}
