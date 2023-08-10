namespace Fanex.NAP.CTS.Model.API.FPS.Account
{
    public class GetAssAccByDevSubscriberItemModel
    {
        public int AccountID            { get; set; }

        public string LoginName         { get; set; }

        public int DeviceInCommon       { get; set; }

        public int OtherDeives          { get; set; }

        public int AssociatedAccount    { get; set; }
    }
}
