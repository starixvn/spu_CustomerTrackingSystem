namespace Fanex.NAP.CTS.Criteria.AssociationDetection
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class DetectAssociationByIPCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustIDList")]
        public string CustIDs { get; set; }

        public override string GetSettingKey()
        {
            return "SPU_AIML_GB_DetectAssociationByIP";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
