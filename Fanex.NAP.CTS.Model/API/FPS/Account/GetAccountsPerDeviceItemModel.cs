namespace Fanex.NAP.CTS.Model.API.FPS.Account
{
    public class GetAccountsPerDeviceItemModel
    {
        public ulong DeviceId { get; set; }

        public string DeviceNo { get; set; }

        public int NoOfAccount { get; set; }
    }
}
