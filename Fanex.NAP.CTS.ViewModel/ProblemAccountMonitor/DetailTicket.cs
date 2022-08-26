using Fanex.NAP.CTS.Resource;
using System;

namespace Fanex.NAP.CTS.ViewModel.ProblemAccountMonitor
{
    public class DetailTicket
    {
        public string Username { get; set; }

        public int SiteId { get; set; }

        public string SiteName { get; set; }

        public int BettypeId { get; set; }

        public string BettypeName { get; set; }

        public string BetId { get; set; }

        public string BetDate { get; set; }

        public string Choice { get; set; }

        public decimal Handicap1 { get; set; }

        public decimal Handicap2 { get; set; }

        public string HtmlChoice { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

        public bool LiveIndicator { get; set; }

        public string LiveBetScore { get; set; }

        public decimal Odds { get; set; }

        public int OddsType { get; set; }

        public decimal Stake { get; set; }

        public decimal Winlost { get; set; }

        public string TicketStatus { get; set; }

        public int TicketStatusId { get; set; }

        public string IP { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyName { get; set; }

        public int HighlightStatus { get; set; }

        public string VoidRequestId { get; set; }

        public string AssociatedGroupName { get; set; }

        public int? GroupId { get; set; }

        private string _GroupName;
        public string GroupName
        {
            get
            {
                return _GroupName;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    _GroupName = ConstsResources.EMPTY;
                }
                else
                {
                    _GroupName = value;
                }
            }
        }
    }
}