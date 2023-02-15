
namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using Fanex.NAP.CTS.Model;
    public class StakeIpPatternDetailViewModel
    {
        public long CoupleID { get; set; }

        public int GroupID { get; set; }

        public int GroupCustID { get; set; }

        public int CountGroupCustID { get; set; }

        public decimal TotalStake { get; set; }
        
        public int TicketsPerMatch { get; set; }

        public IPPatternType3S IPPatternType { get; set; }

        public bool IsStakePattern { get; set; }

        public string Description
        {
            get
            {
                if (IsStakePattern)
                {
                    return $" Total Stake / Tickets per Match: ~{TotalStake} RM / {TicketsPerMatch}";
                }
                switch (this.IPPatternType)
                {
                    case IPPatternType3S.IPShared3Days:
                        return "IP is shared on exact 3 days";
                    case IPPatternType3S.AtLeast3IPsSameDay:
                        return "At least 3 IPs are on same day";
                    case IPPatternType3S.IPAlternative:
                        return "IP Alternative";
                    default:
                        return "";
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

        public string MatchID { get; set; }

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

        public string Stake { get; set; }

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
    }
}
