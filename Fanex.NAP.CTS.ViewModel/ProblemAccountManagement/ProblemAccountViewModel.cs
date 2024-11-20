namespace Fanex.NAP.CTS.ViewModel.ProblemAccountManagement
{
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using Fanex.NAP.CTS.ViewModel.Common;  
    using System.Collections.Generic;

    public class ProblemAccountViewModel
    {
        public ProblemAccountViewModel()
        {
            SiteGroupList = new List<SiteGroupViewModel>();
            CategoryList = new List<CategoryGroupViewModel>();
            RoleList = new List<CustomerRoleViewModel>();
        }

        public List<SiteGroupViewModel> SiteGroupList { get; set; }

        public List<CategoryGroupViewModel> CategoryList { get; set; }

        public List<CustomerRoleViewModel> RoleList { get; set; }
    }
}
