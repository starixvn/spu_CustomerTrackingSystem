namespace Fanex.NAP.CTS.Model.Account.RobotBetting
{
    using Fanex.NAP.CTS.Resource.Consts;
    using System;

    public class RobotUserDetailSceModel
    {
        public long CustID { get; set; }

        public int RobotTypeOrigin { get; set; }

        public int RobotTypeDisplay { get; set; }

        public string TransList { get; set; }

        public DateTime? FirstTransDate { get; set; }

        public DateTime? LastTransDate { get; set; }

        public Decimal? BetDuration { get; set; }

        public UInt64 BetCount { get; set; }

        public UInt64 TotalBetCount { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? ClusterID { get; set; }

        public string LastTransDateToString
        {
            get
            {
                switch (RobotTypeDisplay)
                {
                    case ((int)Consts.RobotDetectionReasonQueryType.ConsecutiveTicket):
                        {
                            if (LastTransDate != null)
                            {
                                return LastTransDate?.ToString(Consts.FormatDatetimeToString_ddMMMHHmmssfff);
                            }

                            break;
                        }
                    case ((int)Consts.RobotDetectionReasonQueryType.UglyStakeTicket):
                        {
                            if (CreatedDate != null)
                            {
                                return CreatedDate?.ToString(Consts.FormatDatetimeToString_ddMMM);
                            }

                            break;
                        }
                }

                return string.Empty;
            }
        }

        public string FirstTransDateToString
        {
            get
            {
                switch (RobotTypeDisplay)
                {
                    case ((int)Consts.RobotDetectionReasonQueryType.ConsecutiveTicket):
                        {
                            if (FirstTransDate != null)
                            {
                                return FirstTransDate?.ToString(Consts.FormatDatetimeToString_ddMMMHHmmssfff);
                            }

                            break;
                        }
                    case ((int)Consts.RobotDetectionReasonQueryType.UglyStakeTicket):
                        {
                            if (CreatedDate != null)
                            {
                                return CreatedDate?.AddDays(-30).ToString(Consts.FormatDatetimeToString_ddMMM);
                            }

                            break;
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