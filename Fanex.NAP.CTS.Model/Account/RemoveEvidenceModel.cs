namespace Fanex.NAP.CTS.Model.Account
{
    using System;

    public class RemoveEvidenceModel
    {
        public Int64 CTSCustId { get; set; }

        public int EvidenceId { get; set; }

        public string Comment { get; set; }

        public int UserId { get; set; }
    }
}
