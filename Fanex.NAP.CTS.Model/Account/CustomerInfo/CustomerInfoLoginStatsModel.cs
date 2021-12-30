namespace Fanex.NAP.CTS.Model.Account.CustomerInfo
{  
    public class CustomerInfoLoginStatsModel
    {
        public int DesktopUsage { get; set; }

        public decimal DesktopUsagePercent { get; set; }

        public int MobileUsage { get; set; }

        public decimal MobileUsagePercent { get; set; }

        public int OthersUsage { get; set; }

        public decimal OthersUsagePercent { get; set; }

        public int Logins { get; set; }

        public int IPs { get; set; }

        public int Devices { get; set; }

    }
}
