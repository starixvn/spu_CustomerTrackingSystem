namespace Fanex.NAP.CTS.Model.API.FPS.Account
{
    using System;

    public class GetAssAccountInfoByCustSubscriberModel
    {
        public int AssociatedDevices { get; set; }

        public int AssociatedAccounts { get; set; }

        public DateTime? FirstLogin { get; set; }

        public string FirstDevice { get; set; }
    }
}
