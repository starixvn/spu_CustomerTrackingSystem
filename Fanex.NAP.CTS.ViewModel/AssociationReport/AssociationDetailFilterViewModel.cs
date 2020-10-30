namespace Fanex.NAP.CTS.ViewModel.AssociationReport
{
    using System;

    public class AssociationDetailFilterViewModel
    {
        public int draw { get; set; }

        public int start { get; set; }

        public int length { get; set; }

        public string UserName { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
     
        public string AssociationType { get; set; }
    }
}
