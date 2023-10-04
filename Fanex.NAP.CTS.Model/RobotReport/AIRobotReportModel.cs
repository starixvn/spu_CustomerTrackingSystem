namespace Fanex.NAP.CTS.Model.RobotReport
{
    using System;

    public class AIRobotReportModel
    {
        public DateTime DateRange { get; set; }

        public string GetMonthlyTime
        {
            get
            {
                return DateRange.ToString("yyyy/MM");
            }
        }

        public string GetDailyTime
        {
            get
            {
                return DateRange.ToString("yyyy/MM/dd");
            }
        }

        public int TI { get; set; }

        public int SCE { get; set; }

        public int TD { get; set; }

        public int OCRD { get; set; }

        public int TotalAccount { get; set; }
    }
}
