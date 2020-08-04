using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.Account.BettingStatistic
{
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
