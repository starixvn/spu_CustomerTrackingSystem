using Newtonsoft.Json;
using System;

namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    public class MMFraudTicketsSummaryModel
    {
        [JsonProperty("MatchId")]
        public Int64 MatchID { get; set; }

        public Int16 DataSource { get; set; }

        [JsonProperty("IsLive")]
        public int Market { get; set; }

        [JsonProperty("Bettype")]
        public int BettypeID { get; set; }

        public int? BetID { get; set; }

        public string FraudTrans { get; set; }
    }
}
