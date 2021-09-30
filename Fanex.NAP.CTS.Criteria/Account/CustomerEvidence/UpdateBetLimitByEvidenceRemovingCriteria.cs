namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data.Repository;

    public class UpdateBetLimitByEvidenceRemovingCriteria : NonQueryCommand
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
