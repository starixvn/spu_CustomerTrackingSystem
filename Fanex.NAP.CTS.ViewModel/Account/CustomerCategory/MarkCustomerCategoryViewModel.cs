namespace Fanex.NAP.CTS.ViewModel.Account.CustomerCategory
{
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class MarkCustomerCategoryViewModel
    {
        public MarkCustomerCategoryViewModel()
        {
            this.ProblemAccountList = new List<ProblemAccountCategoryModel>();
            this.CategoryList = new List<CustomerCategoryViewModel>();
        }

        public bool IsVVip { get; set; }

        public string UserName { get; set; }

        public bool IsRobotUser { get; set; }
       

        public IEnumerable<ProblemAccountCategoryModel> ProblemAccountList { get; set; }

        public IEnumerable<CustomerCategoryViewModel> CategoryList { get; set; }

        public int RoleID { get; set; }               

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
