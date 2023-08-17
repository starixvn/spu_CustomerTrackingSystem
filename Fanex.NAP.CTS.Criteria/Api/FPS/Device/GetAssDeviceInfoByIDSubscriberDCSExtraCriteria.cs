namespace Fanex.NAP.CTS.Criteria.Api.FPS.Device
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssDeviceInfoByIDSubscriberDCSExtraCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_DeviceID")]
        public long DeviceID { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_ET_Association_GetDeviceInfoByDeviceSubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
