namespace Fanex.NAP.CTS.ViewModel.ProblemMatchMonitor
{
    using System.Collections.Generic;

    public class DetailTicketViewModel
    {
        public IEnumerable<DetailTicket> TicketList { get; set; }

        public IEnumerable<DetailSite> SiteList { get; set; }

        public IEnumerable<DetailBetType> BetTypeList { get; set; }

    }
}