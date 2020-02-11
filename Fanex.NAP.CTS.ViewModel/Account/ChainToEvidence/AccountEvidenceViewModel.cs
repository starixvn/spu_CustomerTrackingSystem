namespace Fanex.NAP.CTS.ViewModel.Account
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class AccountEvidenceViewModel
    {
        public AccountEvidenceViewModel()
        {
            EvidenceList = new List<EvidenceViewModel>();
            FlaggedEvidenceList = new List<FlaggedEvidenceViewModel>();
            AffectedRetractEvidenceList = new List<AffectedRetractEvidenceViewModel>();
        }

        public List<EvidenceViewModel> EvidenceList { get; set; }

        public List<FlaggedEvidenceViewModel> FlaggedEvidenceList { get; set; }

        public List<AffectedRetractEvidenceViewModel> AffectedRetractEvidenceList { get; set; }

    }
}
