namespace Fanex.NAP.CTS.Model.Account.CustomerInfo
{  
    public class CustomerInfoLoginStatsModel
    {
        public int BotTrans { get; set; }

        public decimal BotTransPercentage { get; set; }
        
        public int InvalidBrowser { get; set; }

        public decimal InvalidBrowserPercentage { get; set; }

        public int DesktopUsage { get; set; }

        public int MobileUsage { get; set; }

        public int IP { get; set; }

        public int Device { get; set; }

        public int Login { get; set; }
    }
}
