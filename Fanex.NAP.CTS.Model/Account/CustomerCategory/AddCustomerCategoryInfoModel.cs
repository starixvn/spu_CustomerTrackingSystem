namespace Fanex.NAP.CTS.Model.Account.CustomerCategory
{
    public class AddCustomerCategoryInfoModel
    {
        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public int SubscriberID { get; set; }

        public int? CategoryID { get; set; }

        public int RoleID { get; set; }

        public long CreatedBy { get; set; }

        public string Remark { get; set; }

    }
}
