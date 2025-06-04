namespace Fanex.NAP.CTS.Criteria.ManageSubscriber
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ReUpdateSubriberIdBySiteIdCriteira : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_SubsSiteJson", AllowNull = true)]
        public string SubsSiteJson { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CTSCustomer_UpdateSubscriberIDBySiteID";

        public override bool IsValid()
            => true;
    }
}
