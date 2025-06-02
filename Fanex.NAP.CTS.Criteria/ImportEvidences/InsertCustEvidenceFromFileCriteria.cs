namespace Fanex.NAP.CTS.Criteria.ImportEvidences
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertCustEvidenceFromFileCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustomerEvidences")]
        public string CustomerEvidencesJson { get; set; }

        [SpParam(SqlParamName = "ip_IsOverwrite")]
        public bool IsOverwrite { get; set; }

        [SpParam(SqlParamName = "ip_FileName")]
        public string FileName { get; set; }

        [SpParam(SqlParamName = "ip_FileKey")]
        public long FileKey { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy")]
        public int UserId { get; set; }
       
        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustEvidenceFromFile_Insert";

        public override bool IsValid()
            => true;

    }
}
