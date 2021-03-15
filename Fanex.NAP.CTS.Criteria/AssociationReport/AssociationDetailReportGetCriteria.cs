namespace Fanex.NAP.CTS.Criteria.AssociationReport
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class AssociationDetailReportGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public int CTSCustID { get; set; }

        [SpParam(SqlParamName = "ip_AssociationType")]
        public int AssociationType { get; set; }

        [SpParam(SqlParamName = "ip_FromAssDate")]
        public DateTime FromAssDate { get; set; }

        [SpParam(SqlParamName = "ip_ToAssDate")]
        public DateTime ToAssDate { get; set; }

        [SpParam(SqlParamName = "ip_Skip")]
        public int SkipRow { get; set; }

        [SpParam(SqlParamName = "ip_Take")]
        public int TakeRow { get; set; }

        [SpParam(SqlParamName = "op_TotalItems")]
        public int TotalItems { get; set; }

        public override string GetSettingKey()
          => "CTS_DC_RPT_AssociatedAccount_Detail";

        public override bool IsValid()
            => true;
    }
}
