namespace Fanex.NAP.CTS.Model.API.FPS.Account
{
    public class GetDeviceByAccountParamModel
    {
        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public string SubscriberId { get; set; }
    }
}
