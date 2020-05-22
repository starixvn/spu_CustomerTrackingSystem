namespace Fanex.NAP.CTS.Criteria.ProblemMatchMonitor
{
    using Fanex.Data.Repository;

    public class MoveBetBalanceUpDateGetCriteria : CriteriaBase
    {
        public bool IsGetMoveBetDate { get; set; }

        public override string GetSettingKey()
            => "FS_MoveBetBalanceUpDate_Get";

        public override bool IsValid()
            => true;
    }
}