namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using System;

    public class MatchesDetailInfoViewModel
    {
        public Int64 GroupID { get; set; }

        public string Description
        {
            get
            {
                if (!IsGGB)
                {
                    return "The Time Interval is less than 10 seconds";
                }

                if (!string.IsNullOrEmpty(AdditionalInfo))
                {
                    switch (AdditionalInfo)
                    {
                        case "0":
                            return "The Time Interval is less than a second";

                        default:
                            {
                                if (AdditionalInfo.Contains(","))
                                {
                                    return "The Time Intervals are " + AdditionalInfo + " seconds";
                                }

                                return "The Time Interval is " + AdditionalInfo + " seconds";
                            }
                    }
                };

                return string.Empty;
            }
        }

        public string UserName { get; set; }

        public string TransDateString { get; set; }

        public DateTime TransDate { get; set; }

        public string TransId { get; set; }

        public string BetType { get; set; }

        public string BetTeam { get; set; }

        public decimal Stake { get; set; }

        public decimal Odds { get; set; }

        public string LiveScore { get; set; }

        /// <summary>
        /// Intermediated Properties
        /// </summary>
        public bool IsGGB { get; set; }

        public string AdditionalInfo { get; set; }
    }
}