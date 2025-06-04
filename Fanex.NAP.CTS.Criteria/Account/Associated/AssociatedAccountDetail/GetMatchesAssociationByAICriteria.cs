namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetMatchesAssociationByAICriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID1", AllowNull = true)]
        public Int64 CustID1 { get; set; }

        [SpParam(SqlParamName = "ip_CustID2", AllowNull = true)]
        public Int64 CustID2 { get; set; }

        [SpParam(SqlParamName = "ip_GroupList", AllowNull = true)]
        public string GroupList { get; set; }

        [SpParam(SqlParamName = "op_TotalMatchCount", AllowNull = true)]
        public UInt32 TotalMatchCount { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage", AllowNull = true)]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "SPU_AIML_GB_GetAssociationInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
