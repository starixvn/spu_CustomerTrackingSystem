namespace Fanex.NAP.CTS.Library.Criteria.Api
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Library.Core.BaseClasses;

    public class FlaggedAccountDeleteCriteria : CommandBase
    {
        public string EviCode { get; set; }

        public string DirectAccounts { get; set; }

        public string IndirectAccounts { get; set; }

        public override string GetSettingKey()
        {
            return "FS_FlaggedAccount_Delete";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(EviCode);
        }
    }
}