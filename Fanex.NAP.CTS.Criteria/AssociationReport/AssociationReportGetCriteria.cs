namespace Fanex.NAP.CTS.Criteria.AssociationReport
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class AssociationReportGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_AccountStatusIDs")]
        public string AccountStatusIDs { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberIDs")]
        public string SubscriberIDs { get; set; }

        [SpParam(SqlParamName = "ip_AssociationStatus")]
        public string AssociationStatus { get; set; }

        [SpParam(SqlParamName = "ip_FromAssDate")]
        public DateTime FromAssDate { get; set; }

        [SpParam(SqlParamName = "ip_ToAssDate")]
        public DateTime ToAssDate { get; set; }

        [SpParam(SqlParamName = "ip_EvidenceID")]
        public int EvidenceID { get; set; }

        [SpParam(SqlParamName = "ip_AssociationType")]
        public int AssociationType { get; set; }

        [SpParam(SqlParamName = "ip_Skip")]
        public int SkipRow { get; set; }

        [SpParam(SqlParamName = "ip_Take")]
        public int TakeRow { get; set; }

        [SpParam(SqlParamName = "op_TotalItems")]
        public int TotalItems { get; set; }

        public override string GetSettingKey()
           => "CTS_DC_AssociationReport_Get";

        public override bool IsValid()
            => true;
    }
}
