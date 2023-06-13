namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using System;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class MMDetailSearchViewModel
    {
        public MMDetailSearchViewModel() { }

        public MMDetailSearchViewModel(MMDetailSearchViewModel para)
        {
            this.TypeOfTickets = para.TypeOfTickets;
            this.LastScannedSequenceID = para.LastScannedSequenceID;
            this.EventDateToString = para.EventDateToString;
            this.MatchID = para.MatchID;
            this.BetTypeID = para.BetTypeID;
            this.BetID = para.BetID;
            this.Sites = para.Sites;
            this.LiveIndicator = para.LiveIndicator;
        }

        public MatchMonitorQueryType? TypeOfTickets { get; set; }

        public long? LastScannedSequenceID { get; set; }

        public string EventDateToString { get; set; }

        public Int64 MatchID { get; set; }

        public int BetTypeID { get; set; }

        public Int64 BetID { get; set; }

        public string Sites { get; set; }

        public bool LiveIndicator { get; set; }

        public bool IsRefresh { get; set; }

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
            return $"{region}_{nameOfFuntion}_{EventDateToString}_{MatchID}_{BetTypeID}_{BetID}_{LiveIndicator}";
        }

        public string BuildCacheKeyForAllMMTicket(string nameOfFunction, string region = "")
        {
            return BuildCacheKey(nameOfFunction, region) + $"_{LastScannedSequenceID?.ToString() ?? string.Empty}";
        }

        public string BuildCacheKeyWithSite(string nameOfFunction, string region = "", string site = "")
        {
            var value = string.IsNullOrWhiteSpace(site) ? Sites?.ToString() ?? string.Empty : site;
            return BuildCacheKey(nameOfFunction, region) + $"_{value}";
        }
    }
}