namespace Fanex.NAP.CTS.Model.Account
{
    public class CustomerCategoryModel
    {
        public long CustId { get; set; }

        public long CTSCustId { get; set; }

        public int SubscriberId { get; set; }

        public int CategoryId { get; set; }

        public int RoleId { get; set; }

        public bool IsAffectToAllDownlines { get; set; }

        public string Remark { get; set; }

        public bool IsVVip { get; set; }

        public int CreatedBy { get; set; }

        public ActionTypeQueueCustCategory ActionType { get; set; }
    }
}
