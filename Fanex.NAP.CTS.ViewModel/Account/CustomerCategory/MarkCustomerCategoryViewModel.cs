namespace Fanex.NAP.CTS.ViewModel.Account.CustomerCategory
{
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using System.Collections.Generic;

    public class MarkCustomerCategoryViewModel
    {
        public MarkCustomerCategoryViewModel()
        {
            this.ProblemAccountList = new List<ProblemAccountCategoryModel>();
            this.CategoryList = new List<CustomerCategoryViewModel>();
        }

        public bool IsVVip { get; set; }

        public string UserName { get; set; }

        public IEnumerable<ProblemAccountCategoryModel> ProblemAccountList { get; set; }

        public IEnumerable<CustomerCategoryViewModel> CategoryList { get; set; }

    }
}
