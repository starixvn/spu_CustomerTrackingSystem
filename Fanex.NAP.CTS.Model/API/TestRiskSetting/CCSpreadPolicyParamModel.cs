namespace Fanex.NAP.CTS.Model.API.TestRiskSetting
{
    using System;

    public class CCSpreadPolicyParamModel
    {
        public bool IsTest { get; set; }

        public bool IsLive { get; set; }

        public bool IsLicensee { get; set; }

        public DateTime RepUpdateTime { get; set; }

        public long RowNum { get; set; }

        public int MaxRecords { get; set; }
    }
}
