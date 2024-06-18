namespace Fanex.NAP.CTS.ViewModel.Account.CustomerCategory
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account;

    public class CustomerCategoryViewModel
    {
        public CustomerCategoryViewModel(CustomerClassificationModel data)
        {
            this.CategoryId = data.CategoryID;
            this.CategoryType = data.CategoryType;
            this.CategoryGroup = data.CategoryGroup;
            this.CategoryName = data.CategoryName ?? string.Empty;
            this.IsProblemAccount = data.ParentID == 50; 
            this.CategoryColorType = (CategoryColorType)5;
        }

        public int CategoryId { get; set; }

        public int CategoryType { get; set; }        

        public string CategoryName { get; set; }

        public int CategoryGroup { get; set; }

        public bool IsProblemAccount { get; set; }

        //public bool IsPAProbation { get; set; }

        public CategoryColorType CategoryColorType { get; set; }

        public string CategoryColorDesc
        {
            get
            {
                return EnumExtensions.GetStringSlow(CategoryColorType);
            }
        }

    }  
}