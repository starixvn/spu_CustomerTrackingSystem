namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetMatchFromMainInfoCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "MatchIds")]
        public string MatchIDs { get; set; }

        [SpParam(SqlParamName = "FromDateTime")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ToDateTime")]
        public DateTime ToDate { get; set; }      

        [SpParam(SqlParamName = "MatchSection")]
        public string MatchSection { get; set; }

        [SpParam(SqlParamName = "Event")]
        public string Event { get; set; }

        public string SportList { get; set; }

        public override string GetSettingKey()
           => "CTS_Rpt_MatchMonitor_MatchInfo";

        public override bool IsValid()
            => true;
    }
}
