namespace Fanex.NAP.CTS.Model.API.FPS.Device
{
    public class GetAssMatrixByDeviceSubscriberParamModel
    {
        public int SubscriberID { get; set; }

        public long DeviceID { get; set; }

        public int NoOfCustomer { get; set; }

        public int NoOfDevice { get; set; }
    }
}
