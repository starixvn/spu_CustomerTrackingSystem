namespace Fanex.NAP.CTS.ViewModel.Account.RobotBetting
{
    using Fanex.NAP.CTS.Model.Account.RobotBetting;

    public class RobotAiLpDetailViewModel
    {
        public RobotAiLpDetailViewModel(RobotAiLpDetailModel data)
        {
            TransDate = data.TransDate.ToString();
            Device = data.Device;
            RobotTracking = data.RobotTracking;
            DeviceType = data.DeviceType;
            Browser = data.Browser;
            IP = data.IP;
            Domain = data.Domain;
        }

        public string TransDate { get; set; }

        public string Device { get; set; }

        public string RobotTracking { get; set; }

        public string DeviceType { get; set; }

        public string Browser { get; set; }

        public string IP { get; set; }

        public string Domain { get; set; }
    }
}
