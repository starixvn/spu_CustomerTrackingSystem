namespace Fanex.NAP.CTS.Criteria.Api.FPS.Device
{
    using Fanex.Data.Repository;
    using Fanex.Data;
    public class GetAssDevByDevSubscriberDCSCenterCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_DeviceID")]
        public long DeviceID { get; set; }

        [SpParam(SqlParamName = "ip_NoOfRecord")]
        public int NoOfRecord { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Association_GetAssociationDeviceByDeviceSubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
