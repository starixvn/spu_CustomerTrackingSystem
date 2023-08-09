namespace Fanex.NAP.CTS.Criteria.Api.FPS.Search
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetListDeviceByCodeSubscriberDcsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_DeviceCode")]
        public string DeviceCode { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_ET_DeviceInfo_GetByDeviceSubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
