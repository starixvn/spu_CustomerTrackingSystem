namespace Fanex.NAP.CTS.Model.AssociatedGroupMonitor
{
    using System;    

    public class AssociatedGroupMonitorModel
    {
        public int GroupID { get; set; }

        public string GroupName { get; set; }

        public int ABI { get; set; }

        public int TotalAccount { get; set; }

        public string Remark { get; set; }

        public DateTime Created { get; set; }

        public string CreatedBy { get; set; }

    }
}
