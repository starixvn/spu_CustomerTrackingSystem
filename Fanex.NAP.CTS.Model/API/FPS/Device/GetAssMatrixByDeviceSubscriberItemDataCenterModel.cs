namespace Fanex.NAP.CTS.Model.API.FPS.Device
{
    using System;
    public class GetAssMatrixByDeviceSubscriberItemDataCenterModel
    {
        public ulong AccountID { get; set; }

        public string RegisterName { get; set; }

        public string UserName { get; set; }

        public long DeviceID { get; set; }

        public string FirstDeviceCode { get; set; }

        public DateTime? CreatedTime { get; set; }
    }
}
