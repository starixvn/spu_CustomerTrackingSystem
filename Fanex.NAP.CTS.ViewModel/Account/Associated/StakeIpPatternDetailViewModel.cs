
namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Resource.Consts;
    using System;

    public class StakeIpPatternDetailViewModel
    {
        public int GroupID { get; set; }

        public int GroupCustID { get; set; }

        public int CountGroupCustID { get; set; }

        public decimal TotalStake { get; set; }
        
        public int TicketsPerMatch { get; set; }

        public bool IsStakePattern { get; set; }

        public int PatternType { get; set; }

        public string Description
        {
            get
            {
                if (IsStakePattern)
                {
                    return $" Total Stake / Tickets per Match: ~{TotalStake} RM / {TicketsPerMatch}";
                }
                else
                {
                    return _iP;
                }
            }
        }

        private string _userName { get; set; }
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value ?? "";
            }
        }

        public long MatchID { get; set; }

        public string MatchIDToString 
        { 
            get
            {
                return MatchID.ToString();
            }
        }

        public  DateTime TransDate { get; set; }

        private string _transDateString { get; set; }
        public string TransDateString
        {
            get
            {
                return _transDateString;
            }
            set
            {
                _transDateString = value ?? "";
            }
        }

        public string TransId { get; set; }

        private string _betType { get; set; }
        public string BetType
        {
            get
            {
                return _betType;
            }
            set
            {
                _betType = value ?? "";
            }
        }

        private string _betTeam { get; set; }
        public string BetTeam
        {
            get
            {
                return _betTeam;
            }
            set
            {
                _betTeam = value ?? "";
            }
        }

        public string StakeString { get; set; }

        public decimal Stake { get; set; }

        public decimal Odds { get; set; }

        private string _liveScore { get; set; }
        public string LiveScore
        {
            get
            {
                return _liveScore;
            }
            set
            {
                _liveScore = value ?? "";
            }
        }

        //public string StakeRM { get; set; }

        private string _currency { get; set; }
        public string Currency
        {
            get
            {
                return _currency;
            }
            set
            {
                _currency = value ?? "";
            }
        }

        public string _iP { get; set; }
        public string IP
        {
            get
            {
                return _iP;
            }
            set
            {
                _iP = value ?? "";
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
    }
}
