namespace Fanex.NAP.CTS.Model.API.FPS.Device
{
    public class GetDevicesPerAccountItemModel
    {
        public ulong CTSCustID { get; set; }

        public string UserName { get; set; }

        public string RegisterName { get; set; } = string.Empty;

        public int NoOfDevice { get; set; }
    }
}
