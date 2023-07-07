namespace Fanex.NAP.CTS.Criteria.Api.FPS.Device
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssDevicesByCustSubscriberCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustID { get; set; }

        [SpParam(SqlParamName = "ip_NoOfRecord")]
        public int NoOfRecord { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_GetAssociationDeviceByCustSubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
