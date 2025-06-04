namespace Fanex.NAP.CTS.Criteria.ImportEvidences
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertCustEvidenceFromFileCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustomerEvidences", AllowNull = true)]
        public string CustomerEvidencesJson { get; set; }

        [SpParam(SqlParamName = "ip_IsOverwrite", AllowNull = true)]
        public bool IsOverwrite { get; set; }

        [SpParam(SqlParamName = "ip_FileName", AllowNull = true)]
        public string FileName { get; set; }

        [SpParam(SqlParamName = "ip_FileKey", AllowNull = true)]
        public long FileKey { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy", AllowNull = true)]
        public int UserId { get; set; }
       
        [SpParam(SqlParamName = "op_ErrorMessage", AllowNull = true)]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustEvidenceFromFile_Insert";

        public override bool IsValid()
            => true;

    }
}
