namespace Fanex.NAP.CTS.Library.Criteria.FraudCase.ScannedOfficeIP
{    
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Library.Core.BaseClasses;

    public class OfficeIPInternalAccountDeleteCriteria : CommandBase
    {
        public int CustId { get; set; }

        public int UserId { get; set; }

        public bool IsActive { get; set; }

        public override string GetSettingKey()
        {
            return "FS_OfficeIPInternalAccount_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
