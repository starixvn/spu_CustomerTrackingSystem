namespace Fanex.NAP.CTS.ViewModel.Account.CustomerCategory
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account;

    public class GetCustomerCategoryViewModel
    {
        public GetCustomerCategoryViewModel(GetCustomerClassificationModel data)
        {
            this.CategoryId = data.CategoryID;          
            this.CategoryGroup = data.CategoryGroupID;
            this.CategoryName = data.CategoryName ?? string.Empty;         
            this.IsProblemAccount = data.IsPA;
            this.IsVVIP = data.IsVVIP;            
            this.CategoryColorType = (CategoryColorType)data.ColorTypeID;
        }

        public int CategoryId { get; set; }
        
        public string CategoryName { get; set; }

        public int CategoryGroup { get; set; }

        public bool IsProblemAccount { get; set; }     

        public bool IsVVIP { get; set; }      

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