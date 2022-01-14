namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class MatchDetailMonitorGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_MatchID")]
        public Int64 MatchID { get; set; }

        [SpParam(SqlParamName = "ip_BettypeID")]
        public int BettypeID { get; set; }    

        [SpParam(SqlParamName = "ip_BetID")]
        public int? BetID { get; set; }

        [SpParam(SqlParamName = "ip_LiveIndicator")]
        public bool LiveIndicator { get; set; }       
        
        [SpParam(SqlParamName = "ip_Reason")]
        public int Reason { get; set; }

        [SpParam(SqlParamName = "ip_ScoreList")]
        public string ListScore { get; set; }

        [SpParam(SqlParamName = "ip_GroupIDList")]
        public string ListGroup { get; set; }        

        public override string GetSettingKey()
            => "CTS_DC_MatchMonitor_Details_Get";

        public override bool IsValid()
            => true;
    }
}
