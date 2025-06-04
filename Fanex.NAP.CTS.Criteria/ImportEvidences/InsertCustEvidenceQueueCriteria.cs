namespace Fanex.NAP.CTS.Criteria.ImportEvidences
{
    using Fanex.Data;
    using Fanex.Data.Repository;    

    public class InsertCustEvidenceQueueCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustomerEvidences", AllowNull = true)]
        public string CustomerEvidences { get; set; }

        [SpParam(SqlParamName = "ip_FileName", AllowNull = true)]
        public string FileName { get; set; }

        [SpParam(SqlParamName = "ip_FileKey", AllowNull = true)]
        public string FileKey { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy", AllowNull = true)]
        public int CreatedBy { get; set; }

        [SpParam(SqlParamName = "ip_ActionID", AllowNull = true)]
        public int ActionID { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage", AllowNull = true)]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustEvidenceQueue_Insert";

        public override bool IsValid()
            => true;

    }
}
