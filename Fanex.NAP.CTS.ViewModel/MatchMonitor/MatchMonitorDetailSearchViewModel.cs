namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using System;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class MatchMonitorDetailSearchViewModel
    {
        public MatchMonitorQueryType? TypeOfTickets { get; set; }

        public long? LastScannedSequenceID { get; set; }

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

        public string BuildCacheKey(string nameOfFuntion, string region = "")
        {
            return $"{region}_{nameOfFuntion}_{EventDateToString}_{MatchID}_{BetTypeID}_{BetID}_{Sites}_{LiveIndicator}";
        }

        public string BuildCacheKeyForAllMMTicket(string nameOfFunction, string region = "")
        {
            return BuildCacheKey(nameOfFunction, region) + $"_{LastScannedSequenceID?.ToString() ?? string.Empty}";
        }
    }
}