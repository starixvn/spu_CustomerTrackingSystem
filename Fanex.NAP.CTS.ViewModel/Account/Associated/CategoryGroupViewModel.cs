namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using System.Collections.Generic;

    public class CategoryGroupViewModel
    {
        public CategoryGroupViewModel()
        {
            CategoryList = new List<CategoryViewModel>();
        }

        public string CategoryGroupName { get; set; }        

        public IEnumerable<CategoryViewModel> CategoryList { get; set; }
    }
}
