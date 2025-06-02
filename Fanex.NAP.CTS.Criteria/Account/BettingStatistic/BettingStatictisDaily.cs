namespace Fanex.NAP.CTS.Criteria.Account.BettingStatistic
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    public class BettingStatictisDaily : CriteriaBase
    {
        [SpParam(SqlParamName = "CustId")]
        public long CustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_CustomerStatistics_GetPerformanceDaily";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
