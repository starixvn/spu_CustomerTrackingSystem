
namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using Fanex.NAP.CTS.Model;
    using System;

    public class StakeIpPatternDetailViewModel
    {
        public int CoupleID { get; set; }
        public Int64 GroupID { get; set; }
        public Int64 GroupUserNameID { get; set; }
        public int TotalStakePattern { get; set; }
        public IPPatternType IPPatternType { get; set; }
        public bool IsStakePattern { get; set; }
        public string Description
        {
            get
            {
                if (IsStakePattern)
                {
                    return $" Total Stake / Tickets per Match: {StakeRM}RM / {TotalStakePattern}";
                }
                switch (this.IPPatternType)
                {
                    case IPPatternType.IPShared3Days:
                        return "IP is shared on exact 3 days";
                    case IPPatternType.IPAlternative:
                        return "At least 3 IPs are on same day";
                    case IPPatternType.AtLeast3IPsSameDay:
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

        public int MatchID { get; set; }

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

        public long TransId { get; set; }

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

        public decimal StakeRM { get; set; }

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
