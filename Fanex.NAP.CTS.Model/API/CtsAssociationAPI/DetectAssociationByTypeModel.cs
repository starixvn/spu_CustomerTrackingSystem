namespace Fanex.NAP.CTS.Model.API
{
    using System;

    public class DetectAssociationByTypeModel
    {
        public int CustId { get; set; }

        public int AssociationTypeID { get; set; }

        public string AssociationTypeName { get; set; }

        public DateTime LastTicketDate { get; set; }
    }
}
