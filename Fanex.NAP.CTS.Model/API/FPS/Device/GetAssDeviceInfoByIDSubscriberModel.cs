namespace Fanex.NAP.CTS.Model.API.FPS.Device
{
    using System;
    using System.Collections.Generic;

    public class GetAssDeviceInfoByIDSubscriberModel
    {
        public int AssociatedDevices { get; set; }

        public int AssociatedAccounts { get; set; }

        public IEnumerable<GetAssDeviceFirstSeen> FirstSeenBySubscriber { get; set; }
    }

    public class GetAssDeviceFirstSeen
    {
        public int SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
