using static Fanex.NAP.CTS.Resource.Consts.Consts;

namespace Fanex.NAP.CTS.Model.Account
{
    public class CustomerCategoryModel
    {
        public long CustId { get; set; }

        public long CTSCustId { get; set; }

        public int SubscriberId { get; set; }

        public int CategoryId { get; set; }

        public ROLE_CUSTOMER RoleId { get; set; }

        public bool IsAffectToAllDownlines { get; set; }

        public string Remark { get; set; }

        public bool IsVVip { get; set; }

        public int CreatedBy { get; set; }

        public ActionTypeQueueCustCategory ActionType { get; set; }
    }
}
