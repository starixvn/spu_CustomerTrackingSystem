namespace Fanex.NAP.CTS.Model.CustomerClassificationBySport.ProblemAccountInsertBySport
{
    using System;

    public class InsertProblemAccountBySportModel : CustomerClassificationBySportModel
    {
        public InsertProblemAccountBySportModel() { }

        public InsertProblemAccountBySportModel(RescanCustomerClassificationBySportModel model)
        {
            CTSCustID = model.CTSCustID;
            CustID = model.CustID;
            CategoryID = model.CategoryID;
            CategoryGroupID = model.CategoryGroup;
            CreatedBy = model.CreatedBy;
        }

        public Int64 CTSCustID { get; set; }

        public int? RoleID { get; set; }

        public int SubscriberID { get; set; }

        public bool IsLicensee { get; set; }

        public Int64? CreatedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int? WinlossStatus { get; set; }

        public int SportGroup { get; set; }

        public int? MatchID { get; set; }

        public string Remark { get; set; }
    }
}
