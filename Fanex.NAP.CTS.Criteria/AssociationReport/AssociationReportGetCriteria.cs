namespace Fanex.NAP.CTS.Criteria.AssociationReport
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class AssociationReportGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_AccountStatusIDs", AllowNull = true)]
        public string AccountStatusIDs { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberIDs", AllowNull = true)]
        public string SubscriberIDs { get; set; }

        [SpParam(SqlParamName = "ip_AssociationStatus", AllowNull = true)]
        public string AssociationStatus { get; set; }

        [SpParam(SqlParamName = "ip_FromAssDate", AllowNull = true)]
        public DateTime FromAssDate { get; set; }

        [SpParam(SqlParamName = "ip_ToAssDate", AllowNull = true)]
        public DateTime ToAssDate { get; set; }

        [SpParam(SqlParamName = "ip_EvidenceIDs", AllowNull = true)]
        public string EvidenceIDs { get; set; }

        [SpParam(SqlParamName = "ip_AssociationType", AllowNull = true)]
        public int AssociationType { get; set; }

        [SpParam(SqlParamName = "ip_Skip", AllowNull = true)]
        public int SkipRow { get; set; }

        [SpParam(SqlParamName = "ip_Take", AllowNull = true)]
        public int TakeRow { get; set; }

        [SpParam(SqlParamName = "op_TotalItems", AllowNull = true)]
        public int TotalItems { get; set; }

        public override string GetSettingKey()
           => "CTS_DC_RPT_AssociatedAccount";

        public override bool IsValid()
            => true;
    }
}
