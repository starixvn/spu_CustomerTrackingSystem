namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TransGroupModel
    {
        public Int64 TransID { get; set; }

        public string ReasonList { get; set; }

        public string ReasonNameList { get; set; }

        public int GroupName { get; set; }

        public string VerifiedDate { get; set; }
      
        public string VerifiedBy { get; set; }

        public string GetReasonName(IEnumerable<MatchMonitorReasonModel> Reasons, string reason)
        {
            if (!string.IsNullOrEmpty(reason))
            {
                var arrReasons = reason.Split(',').Select(x => Convert.ToInt32(x));
                var reasonNames = Reasons.Join(arrReasons,
                           r1 => r1.ReasonID,
                           r2 => r2,
                           (r1, r2) => r1.ReasonName);

                return string.Join(", ", reasonNames);
            }

            return string.Empty;
        }
    }
}
