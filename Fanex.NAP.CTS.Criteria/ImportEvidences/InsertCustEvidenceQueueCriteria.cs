namespace Fanex.NAP.CTS.Criteria.ImportEvidences
{
    using Fanex.Data;
    using Fanex.Data.Repository;    

    public class InsertCustEvidenceQueueCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustomerEvidences")]
        public string CustomerEvidences { get; set; }

        [SpParam(SqlParamName = "ip_FileName")]
        public string FileName { get; set; }

        [SpParam(SqlParamName = "ip_FileKey")]
        public string FileKey { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy")]
        public int CreatedBy { get; set; }

        [SpParam(SqlParamName = "ip_ActionID")]
        public int ActionID { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustEvidenceQueue_Insert";

        public override bool IsValid()
            => true;

    }
}
