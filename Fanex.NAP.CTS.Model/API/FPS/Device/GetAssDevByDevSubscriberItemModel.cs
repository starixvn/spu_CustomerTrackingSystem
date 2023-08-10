using System;

namespace Fanex.NAP.CTS.Model.API.FPS.Device
{
    public class GetAssDevByDevSubscriberItemModel
    {
        public int          DeviceID            { get; set; }

        public string       DeviceCode          { get; set; }

        public DateTime?    FirstSeenDate       { get; set; }

        public int          AssociatedAccounts  { get; set; }

        public int          AssociatedDevices   { get; set; }
    }
}
