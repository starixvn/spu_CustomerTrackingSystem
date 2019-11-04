namespace Fanex.NAP.CTS.Library.Criteria.Notification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Library.Core.BaseClasses;

    public class RecipientAddCriteria : CommandBase
    {
        public string RecipientIds { get; set; }

        public int UserId { get; set; }
        public int FraudCaseId { get; internal set; }

        public override string GetSettingKey() => "FS_Recipients_Add";

        public override bool IsValid() => true;
    }
}