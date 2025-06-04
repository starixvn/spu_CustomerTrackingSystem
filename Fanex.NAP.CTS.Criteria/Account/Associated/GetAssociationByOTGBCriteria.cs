
namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssociationByOTGBCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID1", AllowNull = true)]
        public long CustId1 { get; set; }

        [SpParam(SqlParamName = "ip_CustID2", AllowNull = true)]
        public long CustId2 { get; set; }        
        
        [SpParam(SqlParamName = "ip_FraudType", AllowNull = true)]
        public int FraudType { get; set; }

        public override string GetSettingKey()
        {
            return "SPU_AIML_OTGB_GetCouple_FraudDetails";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
