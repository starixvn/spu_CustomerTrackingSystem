namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetMatchesAssociationByAICriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID1")]
        public Int64 CustID1 { get; set; }

        [SpParam(SqlParamName = "ip_CustID2")]
        public Int64 CustID2 { get; set; }

        [SpParam(SqlParamName = "ip_GroupList")]
        public string GroupList { get; set; }

        [SpParam(SqlParamName = "op_TotalMatchCount")]
        public UInt32 TotalMatchCount { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
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
