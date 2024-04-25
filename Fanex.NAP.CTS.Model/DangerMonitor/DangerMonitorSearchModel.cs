namespace Fanex.NAP.CTS.Model.DangerMonitor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DangerMonitorSearchModel
    {

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string QueryDanger { get; set; }

        public IEnumerable<string> SportList { get; set; }

        public string Event { get; set; } /* League / Match */

        public string Market { get; set; }

        public string MatchSection { get; set; }

        public string BetTypes { get; set; }

        public bool Validate
        {
            get
            {
                if (FromDate != null && ToDate != null && FromDate > ToDate)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(QueryDanger) || string.IsNullOrEmpty(Market) || string.IsNullOrEmpty(MatchSection))
                {
                    return false;
                }

                if (SportList == null || !SportList.Any())
                {
                    return false;
                }

                if (string.IsNullOrEmpty(BetTypes) || BetTypes.Equals("[]"))
                {
                    return false;
                }

                return true;
            }
        }
    }
}