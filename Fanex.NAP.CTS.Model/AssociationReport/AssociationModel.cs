namespace Fanex.NAP.CTS.Model.AssociationReport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AssociationModel
    {
        public int CTSCustId { get; set; }

        public string UserName { get; set; }

        public int SubscriberId { get; set; }

        public string AccountStatus { get; set; }

        public string EvidenceCode {
            get
            {
                if (EvidenceCodes == null || EvidenceCodes.Any())
                {
                    return string.Join(",", EvidenceCodes);
                }

                return string.Empty;
            }
        }

        public List<string> EvidenceCodes { get; set; }

        public string CategoryNames { get; set; }

        public string SubscriberName { get; set; }
    }
}