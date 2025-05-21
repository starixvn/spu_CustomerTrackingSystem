namespace Fanex.NAP.CTS.Model.API.FPS.Device
{
    public class GetDevicesPerAccountItemModel
    {
        public ulong AccountID { get; set; }

        public string LoginName { get; set; }

        public int NoOfDevice { get; set; }
    }
}
