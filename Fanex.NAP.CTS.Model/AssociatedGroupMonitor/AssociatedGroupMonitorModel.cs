namespace Fanex.NAP.CTS.Model.AssociatedGroupMonitor
{
    using System;    

    public class AssociatedGroupMonitorModel
    {
        public long GroupID { get; set; }

        public string GroupName { get; set; }

        public int ABI { get; set; }        

        public bool AllCredit { get; set; }

        public bool AllLicensee { get; set; }

        public bool HasDevice { get; set; }

        public bool HasManual { get; set; }

        public bool HasIP { get; set; }

        public bool HasAI { get; set; }

        public string Sites { get; set; }

        public int TotalAccount { get; set; }

        public string Remark { get; set; }

        public DateTime Created { get; set; }

        public string CreatedBy { get; set; }

    }
}
