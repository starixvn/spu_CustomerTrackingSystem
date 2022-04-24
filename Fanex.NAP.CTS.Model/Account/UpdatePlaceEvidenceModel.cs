namespace Fanex.NAP.CTS.Model.Account
{
    using System;

    public class UpdatePlaceEvidenceModel
    {
        public Int64 CTSCustId { get; set; }

        public long CustId { get; set; }

        public int SubscriberId { get; set; }

        public int EvidenceId { get; set; }

        public string EvidenceCode { get; set; }

        public string Comment { get; set; }

        public Int64 UserId { get; set; }
    }
}
