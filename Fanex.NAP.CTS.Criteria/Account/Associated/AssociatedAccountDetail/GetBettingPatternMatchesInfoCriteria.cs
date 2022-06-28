namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetMatchesInfoCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "MatchIds")]
        public string MatchIds { get; set; }

        [SpParam(SqlParamName = "CustIds")]
        public string CustIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Rpt_BettingPattern_MatchInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
