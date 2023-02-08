namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetMatchFromMainInfoCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_MatchIDs")]
        public string MatchIDs { get; set; }

        [SpParam(SqlParamName = "ip_FromDate")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate")]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "ip_MatchSection")]
        public string MatchSection { get; set; }

        public override string GetSettingKey()
           => "CTS_Rpt_MatchMonitor_MatchInfos";

        public override bool IsValid()
            => true;
    }
}
