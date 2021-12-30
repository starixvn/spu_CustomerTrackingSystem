namespace Fanex.NAP.CTS.ViewModel.Account.CustomerInfo
{
    public class CustomerInfoLoginStatsViewModel
    {
        public int DesktopUsage { get; set; }

        public string DesktopUsagePercent { get; set; }

        public int MobileUsage { get; set; }

        public string MobileUsagePercent { get; set; }

        public int OthersUsage { get; set; }

        public string OthersUsagePercent { get; set; }

        public int LoginCount { get; set; }

        public int IPCount { get; set; }

        public int DeviceCount { get; set; }
    }
}
