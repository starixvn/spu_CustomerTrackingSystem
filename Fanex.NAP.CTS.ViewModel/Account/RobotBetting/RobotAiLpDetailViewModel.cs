namespace Fanex.NAP.CTS.ViewModel.Account.RobotBetting
{
    using Fanex.NAP.CTS.Model.Account.RobotBetting;
    using Fanex.NAP.CTS.Resource.Consts;

    public class RobotAiLpDetailViewModel
    {
        public RobotAiLpDetailViewModel(RobotAiLpDetailModel data)
        {
            TransTime = FormatGridColumnString(data.TransTime.ToString(Consts.FormatDatetimeToString_ddMMMyyyyHHmmssfff));
            DeviceID = FormatGridColumnString(data.DeviceID);
            RobotTracking = FormatGridColumnString(data.RobotTracking);
            DeviceType = FormatGridColumnString(data.DeviceType);
            BrowserName = FormatGridColumnString(data.BrowserName);
            IP = FormatGridColumnString(data.IP);
            Domain = FormatGridColumnString(data.Domain);
        }

        public string TransTime { get; set; }

        public string DeviceID { get; set; }

        public string RobotTracking { get; set; }

        public string DeviceType { get; set; }

        public string BrowserName { get; set; }

        public string IP { get; set; }

        public string Domain { get; set; }

        private string FormatGridColumnString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "-";
            }

            return value;
        }
    }
}
