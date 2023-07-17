namespace Fanex.NAP.CTS.Model.API.FPS.Device
{
    public class GetAssDevicesByCustSubscriberParamModel
    {
        public int SubscriberID { get; set; }

        public long CTSCustID { get; set; }

        public int NoOfRecord { get; set; }
    }
}
