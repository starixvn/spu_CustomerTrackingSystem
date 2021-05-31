namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Resource.Consts;
    using System;

    public class MatchMonitorModel
    {
        public Int64 ID { get; set; }

        public Int64 MatchID { get; set; }

        public DateTime EventDate { get; set; }

        public int HomeID { get; set; }

        public int AwayID { get; set; }

        public int LeagueID { get; set; }

        public int BettypeID { get; set; }

        public string Reason { get; set; }

        public bool IsVerified { get; set; }

        public DateTime VerifiedDate { get; set; }

        public string VerifiedBy { get; set; }

        public string EventDateToString
        {
            get
            {
                if (EventDate == DateTime.MinValue)
                {
                    return "-";
                }

                return EventDate.ToString(Consts.FormatDateToString_MMMddyyyy);
            }
        }

        public string VerifiedDateToString
        {
            get
            {
                if (VerifiedDate == DateTime.MinValue)
                {
                    return "-";
                }

                return VerifiedDate.ToString(Consts.FormatDateToString_MMMddyyyy);
            }
        }

        public string MatchName
        {
            get
            {
                var rs = "-";
                var HomeName = CachedRef.GetTeamName(HomeID);
                var AwayName = CachedRef.GetTeamName(AwayID);

                if(!string.IsNullOrEmpty(HomeName) || !string.IsNullOrEmpty(AwayName))
                {
                    rs = HomeName + " vs " + AwayName;
                }

                return rs;
            }
        }

        public string LeagueName
        {
            get
            {
                return CachedRef.GetLeagueName(LeagueID);
            }
        }
    }
}
