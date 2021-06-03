namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using System;

    public class MatchMonitorDetailViewModel
    {
        public string EventDateToString { get; set; }

        public Int64 MatchID { get; set; }

        public int HomeID { get; set; }

        public int AwayID { get; set; }

        public int BetTypeID { get; set; }

        public int EventGroup { get; set; }

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

        public string MatchName
        {
            get
            {
                var rs = "-";
                var HomeName = CachedRef.GetTeamName(HomeID);
                var AwayName = CachedRef.GetTeamName(AwayID);

                if (!string.IsNullOrEmpty(HomeName) || !string.IsNullOrEmpty(AwayName))
                {
                    rs = HomeName + " vs " + AwayName;
                }

                return rs;
            }
        }
    }
}