namespace Fanex.NAP.CTS.Criteria.ManageSubscriber
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SubscriberRemoveCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_SubscriberID", AllowNull = true)]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_UserID", AllowNull = true)]
        public long UserID { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage", AllowNull = true)]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_Subscriber_Remove";

        public override bool IsValid()
            => true;
    }
}
