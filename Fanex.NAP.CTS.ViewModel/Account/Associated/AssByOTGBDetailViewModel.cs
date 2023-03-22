
namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using Fanex.NAP.CTS.Resource.Consts;
    using System;

    public class AssByOTGBDetailViewModel
    {
        public int GroupID { get; set; }

        public long MatchID { get; set; }

        public string MatchIDToString
        {
            get
            {
                return MatchID.ToString();
            }
        }

        public DateTime EventDate { get; set; }

        public string EventDateToString
        {
            get
            {
                return EventDate.ToString(Consts.FormatDateToString_MMMddyyyy);
            }
        }

        public string UserName { get; set; }

        public string IP { get; set; }

        public string StakeString { get; set; }

        public decimal Stake { get; set; }

        public string Currency { get; set; }

        public DateTime TransDate { get; set; }

        public string TransDateString
        {
            get
            {
                return TransDate.ToString(Consts.FormatDatetimeToString_YYYYmmddHHmmss);
            }
        }

        public string TransId { get; set; }

        public string BetType { get; set; }

        public string BetTeam { get; set; }

        public decimal Odds { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

        public string LiveScore
        {
            get
            {
                return LiveHomeScore.ToString() + "-" + LiveAwayScore.ToString();
            }
        }
    }
}
