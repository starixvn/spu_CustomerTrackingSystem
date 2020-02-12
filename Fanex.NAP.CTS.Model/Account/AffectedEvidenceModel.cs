namespace Fanex.NAP.CTS.Model.Account
{
    using System;

    public class AffectedEvidenceModel
    {
        public int EvidenceID { get; set; }

        public string EvidenceCode { get; set; }

        public string EvidenceName { get; set; }

        public string EvidenceDesc { get; set; }

        public string LoginID { get; set; }

        public string UserName { get; set; }

        public string AssociationType { get; set; }

        public string Remark { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Creator { get; set; }

        public DateTime LastLoginTime { get; set; }

    }
}