using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    public class MatchReasonGroupModel
    {
        public int GroupID { get; set; }

        public bool IsHedging { get; set; }

        public string ListCustID { get; set; }

        public string ReasonGroupName { get; set; }

        public int TotalScore { get; set; }
    }
}
