namespace Fanex.NAP.CTS.ViewModel.Account
{
    using System;    

    [Serializable]
    public class AffectedRetractEvidenceViewModel
    {
        public int EvidenceID { get; set; }      

        public string EvidenceDisplay { get; set; }

        public string LoginID { get; set; }

        public string UserName { get; set; }

        public string AssociationType { get; set; }

        public DateTime AffectedTime { get; set; }

        public DateTime LastLoginTime { get; set; }

        public bool IsRetract { get; set; }

    }
}