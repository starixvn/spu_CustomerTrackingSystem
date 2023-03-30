namespace Fanex.NAP.CTS.Criteria.AssociationDetection
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class DetectAssociationByIPCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ListCustId")]
        public string ListCustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Association_IpSharedLast3Days";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
