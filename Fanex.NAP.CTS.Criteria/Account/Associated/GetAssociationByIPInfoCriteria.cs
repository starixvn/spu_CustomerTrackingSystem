namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetAssociationByIPInfoCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID1", AllowNull = true)]
        public long CustId1 { get; set; }

        [SpParam(SqlParamName = "ip_CustID2", AllowNull = true)]
        public long CustId2 { get; set; }

        [SpParam(SqlParamName = "op_TotalMatchCount", AllowNull = true)]
        public UInt32 TotalMatchCount { get; set; }

        [SpParam(SqlParamName = "op_TotalIPCount", AllowNull = true)]
        public UInt32 TotalIPCount { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage", AllowNull = true)]
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
