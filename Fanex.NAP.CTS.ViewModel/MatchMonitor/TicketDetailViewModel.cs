namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using System.Collections.Generic;

    public class TicketDetailViewModel
    {
        public IEnumerable<TicketDetailModel> TicketDetails { get; set; }

        public MMDetailMergeGroupViewModel MMDetailMergeGroup { get; set; }

        public int? TotalTicket { get; set; }

        public long? MinSequenceId { get; set; }
    }
}
