namespace Fanex.NAP.CTS.ViewModel.ProblemAccountMonitor
{
    using System.Collections.Generic;
    using System.Linq;

    public class DetailTicketViewModel
    {
        public DetailTicketViewModel() {
            TicketList = Enumerable.Empty<DetailTicket>();
            SiteList = Enumerable.Empty<DetailSite>();
            BetTypeList = Enumerable.Empty<DetailBetType>();
            CurrencyList = Enumerable.Empty<DetailCurrency>();
            TicketStatusList = Enumerable.Empty<DetailTicketStatus>();
        }

        public IEnumerable<DetailTicket> TicketList { get; set; }

        public IEnumerable<DetailSite> SiteList { get; set; }

        public IEnumerable<DetailBetType> BetTypeList { get; set; }

        public IEnumerable<DetailCurrency> CurrencyList { get; set; }

        public IEnumerable<DetailTicketStatus> TicketStatusList { get; set; }

        public IEnumerable<DetailGroup> GroupList { get; set; }

        public int TotalHighlightStatusWhite { get; set; }

    }
}