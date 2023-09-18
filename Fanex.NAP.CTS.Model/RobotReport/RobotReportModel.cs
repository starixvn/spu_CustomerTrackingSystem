namespace Fanex.NAP.CTS.Model.RobotReport
{
    using System;

    public class RobotReportModel
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

        public int AI { get; set; }

        public int IMPERVA { get; set; }

        public int COUNTER { get; set; }

        public int TotalAccount { get; set; }
    }
}
