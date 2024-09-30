namespace Fanex.NAP.CTS.ViewModel.Common
{
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class CustomerRoleViewModel
    {
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

        public int RoleID { get; set; }
    }
}
