using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.Account.BettingStatistic
{
    public class BettingStaisticByBetTypeGetCriteria: CriteriaBase
    {
        [SpParam(SqlParamName = "CustId")]
        public long CustID { get; set; }

        [SpParam(SqlParamName = "GetTop")]
        public long GetTop { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_CustomerStatistics_GetPerformance_ByBettype";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}