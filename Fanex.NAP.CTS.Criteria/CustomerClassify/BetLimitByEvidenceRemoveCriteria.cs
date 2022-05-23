namespace Fanex.NAP.CTS.Criteria.CustomerClassify
{
    using Fanex.Data.Repository;

    public class BetLimitByEvidenceRemoveCriteria : NonQueryCommand
    {
        public string CustIDList { get; set; }
       
        public override string GetSettingKey()
        {
            return "NAP_BLControl_EvidenceUnflagged";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
