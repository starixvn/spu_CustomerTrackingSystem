namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Core.Extentions;
    using System;
    using System.Collections.Generic;

    public class MatchMonitorSearchViewModel
    {
        public int DateType { get; set; } /* #1: InsertDate, 2: EventDate */

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public IEnumerable<MatchMonitorSearchSportItemViewModel> Sports { get; set; }

        public int? Market { get; set; }

        public string MatchSection { get; set; }

        public int? IsVerified { get; set; }

        public IEnumerable<MatchMonitorSearchBetTypeItemViewModel> BetTypes { get; set; }

        public string  Reasons { get; set; }

        public string Event { get; set; } /* League / Match */

        public bool IsValid()
        {
            return  !string.IsNullOrEmpty(Reasons) &&
                    !string.IsNullOrEmpty(MatchSection) &&
                    BetTypes.IsNotNullOrEmpty() &&
                    Sports.IsNotNullOrEmpty() &&
                    IsValidDateRange(FromDate, ToDate);
        }

        private bool IsValidDateRange(DateTime? fromDate, DateTime? toDate)
        {
            if (toDate != null && fromDate != null)
            {
                TimeSpan t = (toDate?.Date - fromDate?.Date) ?? TimeSpan.MaxValue;
                return t.TotalDays <= 6;
            }

            return false;
        }
    }

    public class MatchMonitorSearchSportItemViewModel
    {
        public int? LeagueGroupID { get; set; }

        public int SportTypeID { get; set; }
    }

    public class MatchMonitorSearchBetTypeItemViewModel
    {
        public int BettypeID { get; set; }

        public long BetID { get; set; }
    }
}
