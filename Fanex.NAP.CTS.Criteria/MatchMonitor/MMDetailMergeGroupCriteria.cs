namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class MMDetailMergeGroupCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_TransList")]
        public string TransList { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_MatchMonitor_Details_MergeGroup";

        public override bool IsValid()
            => true;
    }
}
