namespace Fanex.NAP.CTS.Model.Account
{
    public class CustomerClassificationModel
    {
        public int CategoryID { get; set; }

        public int CategoryType { get; set; }

        public string CategoryName { get; set; }

        public int ParentID { get; set; }

        public int CustID { get; set; }       

        public string ParentCategoryName { get; set; } 
        
    }
}
