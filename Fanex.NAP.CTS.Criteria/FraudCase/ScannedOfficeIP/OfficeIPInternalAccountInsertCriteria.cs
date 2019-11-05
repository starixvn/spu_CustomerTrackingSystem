namespace Fanex.NAP.CTS.Criteria.FraudCase.ScannedOfficeIP
{
    using Fanex.NAP.CTS.Core.BaseClasses;

    public class OfficeIPInternalAccountInsertCriteria : CommandBase
    {
        public int UserId { get; set; }

        public string Username { get; set; }        

        public override string GetSettingKey()
        {
            return "FS_OfficeIPInternalAccount_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
