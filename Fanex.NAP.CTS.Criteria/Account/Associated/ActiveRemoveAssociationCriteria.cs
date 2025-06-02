namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ActiveRemoveAssociationCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_FromSubscriberID")]
        public int FromSubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_FromCTSCustID")]
        public long FromCTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_ToSubscriberID")]
        public int ToSubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_ToCTSCustID")]
        public long ToCTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_Remark")]
        public string Remark { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy")]
        public int CreatedBy { get; set; }

        [SpParam(SqlParamName = "ip_IsActionRemove")]
        public bool IsActionRemove { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_UpdateAssociationRemove";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
