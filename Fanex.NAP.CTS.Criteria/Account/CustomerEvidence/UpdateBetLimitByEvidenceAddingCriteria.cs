namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data.Repository;

    public class UpdateBetLimitByEvidenceAddingCriteria : NonQueryCommand
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
