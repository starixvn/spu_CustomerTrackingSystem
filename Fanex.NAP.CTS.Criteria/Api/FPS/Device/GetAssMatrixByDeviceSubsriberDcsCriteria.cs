namespace Fanex.NAP.CTS.Criteria.Api.FPS.Device
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssMatrixByDeviceSubsriberDcsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_DeviceID")]
        public long DeviceID { get; set; }

        [SpParam(SqlParamName = "ip_NoOfCustomer")]
        public int NoOfCustomer { get; set; }

        [SpParam(SqlParamName = "ip_NoOfDevice")]
        public int NoOfDevice { get; set; }

        public override string GetSettingKey() => "DCS_ET_Association_GetAssociationMatrixByDeviceSubscriber";

        public override bool IsValid() => true;
    }
}
