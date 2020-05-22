namespace Fanex.NAP.CTS.Criteria.ProblemMatchMonitor
{
    using Fanex.Data.Repository;

    public class SportListGetCriteria : CriteriaBase
    {
        public override string GetSettingKey()
            => "FS_CTSSportList_Get";

        public override bool IsValid()
            => true;

    }
}