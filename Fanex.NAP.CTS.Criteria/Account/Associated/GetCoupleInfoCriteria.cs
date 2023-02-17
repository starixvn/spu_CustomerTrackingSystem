
namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetCoupleInfoCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID1")]
        public long CustId1 { get; set; }

        [SpParam(SqlParamName = "ip_CustID2")]
        public long CustId2 { get; set; }

        [SpParam(SqlParamName = "op_FirstCreatedDate")]
        public DateTime FirstCreatedDate { get; set; }

        public override string GetSettingKey()
        {
            return "SPU_AIML_GB_3S_GetCoupleInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
