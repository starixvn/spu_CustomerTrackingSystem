namespace Fanex.NAP.CTS.Model.Account
{   
    public class RemoveCustomerCategoryModel
    {
        public long CustId { get; set; }

        public long CTSCustId { get; set; }

        public int CategoryId { get; set; }

        public bool IsVVip { get; set; }

        public bool IsAffectToAllDownlines { get; set; }

        public int CreatedBy { get; set; }
    }
}
