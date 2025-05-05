namespace Fanex.NAP.CTS.Model.API.TestRiskSetting
{
    using System;

    public class CCPeakHourRangeEntity
    {
        public int WeekDay { get; set; }

        public TimeSpan StartRange { get; set; }

        public TimeSpan EndRange { get; set; }
    }
}
