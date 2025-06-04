namespace Fanex.NAP.CTS.Criteria.AssociationReport
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class AssociationDetailReportGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID", AllowNull = true)]
        public int CTSCustID { get; set; }

        [SpParam(SqlParamName = "ip_AssociationType", AllowNull = true)]
        public int AssociationType { get; set; }

        [SpParam(SqlParamName = "ip_FromAssDate", AllowNull = true)]
        public DateTime FromAssDate { get; set; }

        [SpParam(SqlParamName = "ip_ToAssDate", AllowNull = true)]
        public DateTime ToAssDate { get; set; }

        [SpParam(SqlParamName = "ip_Skip", AllowNull = true)]
        public int SkipRow { get; set; }

        [SpParam(SqlParamName = "ip_Take", AllowNull = true)]
        public int TakeRow { get; set; }

        [SpParam(SqlParamName = "op_TotalItems", AllowNull = true)]
        public int TotalItems { get; set; }

        public override string GetSettingKey()
          => "CTS_DC_RPT_AssociatedAccount_Detail";

        public override bool IsValid()
            => true;
    }
}
