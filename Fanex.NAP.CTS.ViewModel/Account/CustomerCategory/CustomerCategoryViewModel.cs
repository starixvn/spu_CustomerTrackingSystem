namespace Fanex.NAP.CTS.ViewModel.Account.CustomerCategory
{    
    public class CustomerCategoryViewModel
    {
        public int CategoryId { get; set; }

        public int CategoryType { get; set; }        

        public string CategoryName { get; set; }

        public int CategoryGroup { get; set; }

        public bool IsProblemAccount { get; set; }

        public bool IsPAProbation { get; set; }

    }  
}