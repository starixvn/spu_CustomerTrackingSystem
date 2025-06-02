namespace Fanex.NAP.CTS.Model.API.FPS.Account
{
    public class GetAssAccByDevSubscriberItemDCSCenterModel
    {
        public int AccountID { get; set; }

        public string RegisterName { get; set; }

        public string UserName { get; set; }

        public int DeviceInCommon { get; set; }

        public int OtherDeives { get; set; }

        public int AssociatedAccount { get; set; }
    }
}
