namespace Fanex.NAP.CTS.Criteria.Common
{
    using Fanex.Data.Repository;

    public class GetCommonLeagueGroupCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "CTS_LeagueGroup_Get";

        public override bool IsValid() => true;
    }
}
