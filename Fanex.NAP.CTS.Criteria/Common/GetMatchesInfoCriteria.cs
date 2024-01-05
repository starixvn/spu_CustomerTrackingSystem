namespace Fanex.NAP.CTS.Criteria.Common
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetMatchesInfoCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "MatchIds")]
        public string MatchIds { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string SportList { get; set; }

        public string MatchSectionList { get; set; }

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
