namespace Fanex.NAP.CTS.Model.Account.RobotBetting
{
    using Fanex.NAP.CTS.Resource.Consts;
    using System;

    public class RobotUserDetailSceModel
    {
        public long CustID { get; set; }

        public int PeriodRangeType { get; set; }

        public string TransList { get; set; }

        public DateTime? FirstTransDate { get; set; }

        public DateTime? LastTransDate { get; set; }

        public Decimal? BetDuration { get; set; }

        public UInt64 BetCount { get; set; }

        public UInt64 TotalBetCount { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string LastTransDateToString
        {
            get
            {
                if (LastTransDate != null)
                {
                    switch (PeriodRangeType)
                    {
                        case ((int)Consts.RobotDetectionReasonQueryType.ConsecutiveTicket): return LastTransDate?.ToString(Consts.FormatDatetimeToString_ddMMMHHmmssfff);
                        case ((int)Consts.RobotDetectionReasonQueryType.UglyStakeTicket): return LastTransDate?.ToString(Consts.FormatDatetimeToString_ddMMM);
                    }
                }

                return string.Empty;
            }
        }

        public string FirstTransDateToString
        {
            get
            {
                if (FirstTransDate != null)
                {
                    switch (PeriodRangeType)
                    {
                        case ((int)Consts.RobotDetectionReasonQueryType.ConsecutiveTicket): return FirstTransDate?.ToString(Consts.FormatDatetimeToString_ddMMMHHmmssfff);
                        case ((int)Consts.RobotDetectionReasonQueryType.UglyStakeTicket): return FirstTransDate?.ToString(Consts.FormatDatetimeToString_ddMMM);
                    }
                }

                return string.Empty;
            }
        }

        public string TotalHours
        {
            get
            {
                if (BetDuration != null)
                {
                    return Math.Round(BetDuration ?? 0, 2).ToString();
                }

                return string.Empty;
            }
        }

        public string CreatedDateToString
        {
            get
            {
                if (CreatedDate != null)
                {
                    return CreatedDate?.ToString(Consts.FormatDateTimeToString_YYYYmmdd);
                }

                return string.Empty;
            }
        }
    }
}