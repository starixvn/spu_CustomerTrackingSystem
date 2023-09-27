namespace Fanex.NAP.CTS.Model.RobotReport
{
    using System;

    public class RobotReportParamModel
    {
        private DateTime _fromDate { get; set; }

        private DateTime _toDate { get; set; }

        public bool IsMonthly { get; set; }

        public bool IsDaily { get; set; }

        public DateTime FromDate
        {
            get
            {
                if (IsMonthly)
                {
                    return new DateTime(_fromDate.Year, _fromDate.Month, 1);
                }

                return _fromDate;
            }
            set { _fromDate = value; }
        }

        public DateTime ToDate
        {
            get
            {
                if (IsMonthly)
                {
                    return new DateTime(_toDate.Year, _toDate.Month, 1).AddMonths(1).AddSeconds(-1);
                }

                return new DateTime(_toDate.Year, _toDate.Month, _toDate.Day).AddDays(1).AddSeconds(-1);
            }
            set { _toDate = value; }
        }

        public string Sites { get; set; }
    }
}
