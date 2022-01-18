namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using System;

    public class MatchMonitorDetailFilterViewModel
    {
        public string EventDateToString { get; set; }

        public Int64 MatchID { get; set; }

        public int BetTypeID { get; set; }

        public int BetID { get; set; }

        public string Sites { get; set; }

        public int Reason { get; set; }

        public bool LiveIndicator { get; set; }

        public string ListScore { get; set; }

        public string ListGroup { get; set; }

        public string BetChoice { get; set; }

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
    }
}