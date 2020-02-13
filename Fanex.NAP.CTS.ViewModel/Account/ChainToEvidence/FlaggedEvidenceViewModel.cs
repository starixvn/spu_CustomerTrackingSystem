namespace Fanex.NAP.CTS.ViewModel.Account
{
    using System;

    [Serializable]
    public class FlaggedEvidenceViewModel
    {
        public int EvidenceID { get; set; }

        public string EvidenceCode { get; set; }

        public string EvidenceName { get; set; }

        public string EvidenceDisplay { get; set; }

        public string FlaggedDate { get; set; }

        public string Comment { get; set; }

        public string Creator { get; set; }
    }
}