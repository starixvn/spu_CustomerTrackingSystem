namespace Fanex.NAP.CTS.Model.API.FPS.Device
{
    using System;

    public class GetAssDevicesByCustSubscriberItemModel
    {
        public int DeviceID { get; set; }

        public string DeviceNo { get; set; }

        public string DeviceCode { get; set; }

        public int PuluginId { get; set; }

        public string Browser { get; set; }

        public int SubscriberID { get; set; }

        public DateTime LastAccessedDate { get; set; }

        public DateTime FirstSeenDate { get; set; }

        public int AssAcc { get; set; }

        public int AssDev { get; set; }

        public int AccEx { get; set; }
    }
}
