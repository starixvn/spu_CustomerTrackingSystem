namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RobotStatusByTransGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_TransList")]
        public string TransJson { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_RobotClassification_GetRobotStatusByTransList";

        public override bool IsValid()
            => true;
    }
}
