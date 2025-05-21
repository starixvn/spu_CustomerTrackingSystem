namespace Fanex.NAP.CTS.Model.API.FPS.Account
{
    public class GetDevicesPerAccountParamModel
    {
        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public int SubscriberId { get; set; }

        public int TimeRange { get; set; }

        public int NoOfDeivce { get; set; }
    }
}
