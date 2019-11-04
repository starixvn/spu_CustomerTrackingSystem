namespace Fanex.NAP.CTS.Library.Criteria.FraudCase
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Library.Core.BaseClasses;

    public class UpdateFraudCaseStatusCriteria : CommandBase
    {
        public int FraudCaseId { get; set; }

        public int UserId { get; set; }

        public override string GetSettingKey()
        {
            return "FS_FraudCase_CaseStatus_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}