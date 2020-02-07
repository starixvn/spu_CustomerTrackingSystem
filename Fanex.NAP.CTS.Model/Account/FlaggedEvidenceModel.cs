namespace Fanex.NAP.CTS.Model.Account
{
    using System;

    public class FlaggedEvidenceModel
    {
        public int EvidenceID { get; set; }

        public string EvidenceCode { get; set; }

        public string EvidenceName { get; set; }

        public string EvidenceDesc { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Level { get; set; }

        public string Remark { get; set; }

        public string Creator { get; set; }
    }
}