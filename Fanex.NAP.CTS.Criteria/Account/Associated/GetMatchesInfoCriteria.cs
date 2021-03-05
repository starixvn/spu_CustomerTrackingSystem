namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data.Repository;

    public class GetMatchesInfoCriteria : CriteriaBase
    {
        public string MatchIds { get; set; }

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
