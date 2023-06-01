namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using System;

    public class MatchMonitorDetailFilterViewModel
    {
        public int start { get; set; }

        public int length { get; set; }

        public string EventDateToString { get; set; }

        public Int64 MatchID { get; set; }

        public int BetTypeID { get; set; }

        public Int64 BetID { get; set; }

        public string Sites { get; set; }

        public bool LiveIndicator { get; set; }
      
        public DateTime EventDate
        {
            get
            {
                var eventDate = DateTime.MinValue;

                if (!string.IsNullOrEmpty(EventDateToString))
                {
                    return DateTime.TryParse(EventDateToString, out eventDate) ? eventDate : DateTime.MinValue;
                }

                return eventDate;
            }
        }

        public string BuildCacheKey(string nameOfFuntion)
        {
            return $"{nameOfFuntion}_{EventDateToString}_{MatchID}_{BetTypeID}_{BetID}_{Sites}_{LiveIndicator}";
        }
    }
}