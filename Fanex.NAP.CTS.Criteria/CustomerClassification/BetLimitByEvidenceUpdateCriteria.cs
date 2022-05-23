namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data.Repository;

    public class BetLimitByEvidenceUpdateCriteria : NonQueryCommand
    {
        public string CustIDList { get; set; }
       
        public override string GetSettingKey()
        {
            return "NAP_BLControl_EvidenceFlagged";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
