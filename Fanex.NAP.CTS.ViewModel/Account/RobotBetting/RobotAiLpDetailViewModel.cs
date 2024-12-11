namespace Fanex.NAP.CTS.ViewModel.Account.RobotBetting
{
    using Fanex.NAP.CTS.Model.Account.RobotBetting;
    using Fanex.NAP.CTS.Resource.Consts;

    public class RobotAiLpDetailViewModel
    {
        public RobotAiLpDetailViewModel(RobotAiLpDetailModel data)
        {
            TransTime = data.TransTime.ToString(Consts.FormatDatetimeToString_ddMMMyyyyHHmmssfff);
            DeviceID = data.DeviceID;
            RobotTracking = data.RobotTracking;
            DeviceType = data.DeviceType;
            BrowserName = data.BrowserName;
            IP = data.IP;
            Domain = data.Domain;
        }

        public string TransTime { get; set; }

        public string DeviceID { get; set; }

        public string RobotTracking { get; set; }

        public string DeviceType { get; set; }

        public string BrowserName { get; set; }

        public string IP { get; set; }

        public string Domain { get; set; }
    }
}
