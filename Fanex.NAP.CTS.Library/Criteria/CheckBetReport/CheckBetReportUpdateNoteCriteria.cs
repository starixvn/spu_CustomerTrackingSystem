namespace Fanex.NAP.CTS.Library.Criteria.CheckBetReport
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Library.Core.BaseClasses;

    public class CheckBetReportUpdateNoteCriteria : CommandBase
    {
        public long TransId { get; set; }

        public int UserId { get; set; }

        public string Note { get; set; }

        public override string GetSettingKey() => "FS_Rpt_CheckBetNote_Update";

        public override bool IsValid() => true;
    }
}