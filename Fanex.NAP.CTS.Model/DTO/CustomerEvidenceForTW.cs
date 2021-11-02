namespace Fanex.NAP.CTS.Model.DTO
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using System;

    public class CustomerEvidenceForTW
    {
        public int Level0CustId { get; set; }

        public string EvidenceCode { get; set; }

        public int AssociatedLevel { get; set; }
    }
}