namespace Fanex.NAP.CTS.ViewModel.AssociatedGroupMonitor
{
    using System;   

    public class AssociatedGroupMonitorViewModel
    {
        public int GroupID { get; set; }

        public string GroupName { get; set; }

        public int ABI { get; set; }

        public int TotalAccount { get; set; }

        public string Remark { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }
    }
}
