namespace Fanex.NAP.CTS.Model.Account
{
    using System.Collections.Generic;

    public class AffectedRetractEvidenceModel
    {
        public List<AffectedEvidenceModel> AffectedEvidenceList { get; set; }

        public List<AffectedEvidenceModel> RetractEvidenceList { get; set; }

    }
}