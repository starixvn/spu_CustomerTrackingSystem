namespace Fanex.NAP.CTS.Criteria.Api
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core.BaseClasses;

    public class FlaggedAccountInsertCriteria : CommandBase
    {
        public string EviCode { get; set; }

        public string DirectAccounts { get; set; }

        public string IndirectAccounts { get; set; }

        public override string GetSettingKey()
        {
            return "FS_FlaggedAccount_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(EviCode);
        }
    }
}