using System;

namespace Fanex.NAP.CTS.ViewModel.Account
{
    [Serializable]
    public class AffectedEvidenceViewModel
    {
        public int EvidenceID { get; set; }

        public string EvidenceCode { get; set; }

        public string EvidenceName { get; set; }

        public string EvidenceDesc { get; set; }

        public string LoginID { get; set; }

        public string UserName { get; set; }

        public string AssociationType { get; set; }

        public DateTime AffectedTime { get; set; }

        public DateTime LastLoginTime { get; set; }

        public bool IsRetract { get; set; }
    }
}