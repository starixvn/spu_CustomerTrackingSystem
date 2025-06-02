namespace Fanex.NAP.CTS.Criteria.Account.BettingStatistic
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    public class BettingPerformanceBySportGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "CustId")]
        public long CustID { get; set; }

        [SpParam(SqlParamName = "GetTop")]
        public long GetTop { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_CustomerStatistics_GetPerformance_BySport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}