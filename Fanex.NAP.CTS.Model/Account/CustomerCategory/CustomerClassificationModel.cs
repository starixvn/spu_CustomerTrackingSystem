namespace Fanex.NAP.CTS.Model.Account
{
    public class CustomerClassificationModel
    {
        public int CategoryID { get; set; }

        public int CategoryType { get; set; }

        public string CategoryName { get; set; }

        public int CategoryGroupID { get; set; }

        public int ColorTypeID { get; set; }

        public int ParentID { get; set; }

        public int CustID { get; set; }

        public string ParentCategoryName { get; set; }

        public bool IsProbation { get; set; }

        public bool IsVVIP { get; set; }

        public bool IsPA { get; set; }

        public bool IsPotentialPA { get; set; }

    }
}
