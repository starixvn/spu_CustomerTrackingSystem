namespace Fanex.NAP.CTS.Model.API.FPS.Account
{
    public class GetAssAccountsByCustSubscriberItemDcsModel
    {
        public long AccountID { get; set; }

        public string LoginName { get; set; }

        public int DeviceInCommon { get; set; }

        public int OtherDeives { get; set; }

        public int AssociatedAccount { get; set; }
    }
}
