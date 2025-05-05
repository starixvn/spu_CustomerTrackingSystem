namespace Fanex.NAP.CTS.Model.API.TestRiskSetting
{
    using System;

    public class CCSpreadPolicyDetailEntity
    {
        public int PolicyID { get; set; }

        public int CustomerClass { get; set; }

        public int SiteID { get; set; }

        public int Currency { get; set; }

        public int Platform { get; set; }

        public int SportType { get; set; }

        public int LeagueGroupID { get; set; }

        public decimal Spread { get; set; }

        public bool IsLive { get; set; }

        public DateTime RepUpdateTime { get; set; }

        public long RowNum { get; set; }
    }
}
