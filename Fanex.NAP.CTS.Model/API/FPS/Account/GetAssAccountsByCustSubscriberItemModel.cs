namespace Fanex.NAP.CTS.Model.API.FPS.Account
{
    public class GetAssAccountsByCustSubscriberItemModel
    {
        public long CTSCustID { get; set; }

        public string UserName { get; set; }

        public string RegisterName { get; set; }

        public int DeviceInCommon { get; set; }

        public int OtherDeives { get; set; }

        public int AssociatedAccount { get; set; }
    }
}
