namespace Fanex.NAP.CTS.Model.GraphDB
{
    using System;

    public class AssociationDetailReportParameterModel
    {
        public long CTSCustId { get; set; }

        public DateTime AssociatedDateFrom { get; set; }

        public DateTime AssociatedDateTo { get; set; }

        public int AssociationStatus { get; set; }

        public int AssociationType { get; set; }

        public int TakeRow { get; set; }

        public int SkipRow { get; set; }
    }
}
