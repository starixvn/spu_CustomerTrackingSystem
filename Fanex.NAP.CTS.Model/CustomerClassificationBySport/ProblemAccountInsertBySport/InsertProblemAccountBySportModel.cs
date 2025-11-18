namespace Fanex.NAP.CTS.Model.CustomerClassificationBySport.ProblemAccountInsertBySport
{
    using Fanex.NAP.CTS.Model.CustomerClassificationBySport.RescanCustomerClassificationBySport;
    using System;

    public class InsertProblemAccountBySportModel : CustomerClassificationBySportModel
    {
        public InsertProblemAccountBySportModel() { }

        public InsertProblemAccountBySportModel(RescanCustomerClassificationBySportResultModel model)
        {
            CTSCustID = model.CTSCustID;
            CustID = model.CustID;
            CategoryID = model.CategoryID;
            CreatedBy = model.CreatedBy;
            SportGroup = model.SportID;
            Remark = model.Remark;
            CreatedDate = model.CreatedDate;
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

        public DateTime? CreatedDate { get; set; }
    }
}
