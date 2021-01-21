namespace Fanex.NAP.CTS.Criteria.ManageSubscriber
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ReUpdateSubriberIdBySiteIdCriteira : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubsSiteJson")]
        public string SubsSiteJson { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CTSCustomer_UpdateSubsciberIDBySiteID";

        public override bool IsValid()
            => true;
    }
}
