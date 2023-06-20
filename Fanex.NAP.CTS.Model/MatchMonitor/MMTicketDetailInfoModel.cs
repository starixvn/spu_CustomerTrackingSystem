using Fanex.Data;
using System;

namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    public class MMTicketDetailInfoModel
    {
        public long? MaxSequenceId { get; set; }

        public Int64 TotalTicketAll { get; set; }

        public Int64 TotalTicketLicensee { get; set; }

        public Int64 TotalTicketCredit { get; set; }

        public MMDetailFilterInfoModel FilterDetail { get; set; }
    }
}
