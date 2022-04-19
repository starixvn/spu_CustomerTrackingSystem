namespace Fanex.NAP.CTS.Model.Account
{
    using System;

    public class GetTicketDetailParaModel
    {
        public string CustIds { get; set; }

        public DateTime? EventDate { get; set; }

        public int MatchId { get; set; }

        public string Bettypes { get; set; }
    }
}
