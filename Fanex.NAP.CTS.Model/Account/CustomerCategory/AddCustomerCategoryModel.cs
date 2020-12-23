namespace Fanex.NAP.CTS.Model.Account
{
    public class AddCustomerCategoryModel
    {
        public long CustId { get; set; }

        public long CTSCustId { get; set; }

        public int SubscriberId { get; set; }

        public int CategoryId { get; set; }

        public string Remark { get; set; }

        public bool IsVVip { get; set; }
    }
}
