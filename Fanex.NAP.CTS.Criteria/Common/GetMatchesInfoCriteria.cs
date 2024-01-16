namespace Fanex.NAP.CTS.Criteria.Common
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetMatchesInfoCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "MatchIds")]
        public string MatchIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_MatchInfo_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
