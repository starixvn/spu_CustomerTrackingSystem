namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    using System;

    public class ProblemAccountCategoryModel
    {
        public Int64 CustID { get; set; }

        public int CategoryID { get; set; }

        public Int64? CreatedBy { get; set; }
    }
}