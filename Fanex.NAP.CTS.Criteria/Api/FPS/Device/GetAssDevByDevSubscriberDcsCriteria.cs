namespace Fanex.NAP.CTS.Criteria.Api.FPS.Device
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssDevByDevSubscriberDcsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_DeviceID")]
        public int DeviceID { get; set; }

        [SpParam(SqlParamName = "ip_NoOfRecord")]
        public int NoOfRecord { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_ET_Association_GetAssociationDeviceByDeviceSubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
