namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ActiveRemoveAssociationCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_FromSubscriberID", AllowNull = true)]
        public int FromSubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_FromCTSCustID", AllowNull = true)]
        public long FromCTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_ToSubscriberID", AllowNull = true)]
        public int ToSubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_ToCTSCustID", AllowNull = true)]
        public long ToCTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_Remark", AllowNull = true)]
        public string Remark { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy", AllowNull = true)]
        public int CreatedBy { get; set; }

        [SpParam(SqlParamName = "ip_IsActionRemove", AllowNull = true)]
        public bool IsActionRemove { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage", AllowNull = true)]
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
