namespace Fanex.NAP.CTS.Model.API.FPS.Device
{
    using System;

    public class GetAssMatrixByDeviceSubscriberItemModel
    {
        public int AccountID { get; set; }

        public string LoginName { get; set; }

        public long DeviceID { get; set; }

        public string FirstDeviceCode { get; set; }

        public DateTime? CreatedTime { get; set; }
    }
}
