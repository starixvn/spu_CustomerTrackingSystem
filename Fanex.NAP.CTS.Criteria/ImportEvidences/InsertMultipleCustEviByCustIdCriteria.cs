namespace Fanex.NAP.CTS.Criteria.ImportEvidences
{
    using Fanex.Data.Repository;
    using Fanex.Data;

    public class InsertMultipleCustEviByCustIdCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_NewAccountEvidences")]
        public string NewAccountEvidences { get; set; }

        [SpParam(SqlParamName = "ip_UserID")]
        public int UserId { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey() => "CTS_DC_CustEvidence_InsertMultiple_ByCustId";

        public override bool IsValid() => true;
    }
}
