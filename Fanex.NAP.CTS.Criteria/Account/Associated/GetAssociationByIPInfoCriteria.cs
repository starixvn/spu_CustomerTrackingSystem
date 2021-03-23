namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetAssociationByIPInfoCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID1")]
        public long CustId1 { get; set; }

        [SpParam(SqlParamName = "ip_CustID2")]
        public long CustId2 { get; set; }

        [SpParam(SqlParamName = "op_TotalMatchCount")]
        public UInt32 TotalMatchCount { get; set; }

        [SpParam(SqlParamName = "op_TotalIPCount")]
        public UInt32 TotalIPCount { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "SPU_AIML_GetAssociationByIPInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
