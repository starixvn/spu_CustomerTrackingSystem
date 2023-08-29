namespace Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn
{
    using Fanex.NAP.CTS.Model;

    public class TicketStatusViewModel
    {
        public TicketStatusViewModel() { }

        public int TicketStatus { get; set; }

        public string TicketStatusName { get; set; }

        public int StatusOrderIdx 
        { 
            get
            {
                switch (this.TicketStatus)
                {
                    case (int)StatusTicket.Reject:
                        return 1;
                    case (int)StatusTicket.Void:
                        return 2;
                    case (int)StatusTicket.Running:
                        return 3;
                    case (int)StatusTicket.Waiting:
                        return 4;
                    case (int)StatusTicket.Won:
                        return 5;
                    case (int)StatusTicket.Draw:
                        return 6;
                    case (int)StatusTicket.Lose:
                        return 7;
                    case (int)StatusTicket.Refund:
                        return 8;
                    case (int)StatusTicket.Completed:
                        return 9;
                }

                return 0;
            } 
        }
    }
}
