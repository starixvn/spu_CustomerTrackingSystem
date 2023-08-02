namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using System;
    using System.Collections.Generic;

    public class MMTicketDetailInfoModel
    {
        public long? MaxSequenceId { get; set; }

        public Int64 TotalTicketAll { get; set; }

        public Int64 TotalTicketLicensee { get; set; }

        public Int64 TotalTicketCredit { get; set; }

        public IEnumerable<MMDetailFilterTicketInfoModel> TicketInfoList { get; set; }

    }
}
