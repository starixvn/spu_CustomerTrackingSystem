namespace Fanex.NAP.CTS.Criteria.AssociationDetection
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCoupleSharedIPCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustID1")]
        public long CustID1 { get; set; }

        [SpParam(SqlParamName = "ip_CustID2")]
        public long CustID2 { get; set; }

        public override string GetSettingKey()
        {
            return "SPU_AIML_GB_GetCoupleSharedIP";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
