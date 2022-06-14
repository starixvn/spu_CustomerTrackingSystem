using static Fanex.NAP.CTS.Resource.Consts.Consts;

namespace Fanex.NAP.CTS.ViewModel.Account.CustomerCategory
{
    public class UnmarkCustomerCategoryViewModel
    {
        public int RoleID { get; set; }

        public bool IsVVip { get; set; }

        public bool IsLicensee { get; set; }

        public string UserName { get; set; }

        public string RoleName
        {
            get
            {
                switch (this.RoleID)
                {
                    case (int)ROLE_CUSTOMER.MEMBER:
                        return "Member";
                    case (int)ROLE_CUSTOMER.AGENT:
                        return "Agent";
                    case (int)ROLE_CUSTOMER.MASTER:
                        return "Master";
                    case (int)ROLE_CUSTOMER.SUPER:
                        return "Super";
                }

                return "Others";
            }
        }
    }
}
