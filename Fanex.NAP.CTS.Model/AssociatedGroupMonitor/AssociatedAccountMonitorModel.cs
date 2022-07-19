namespace Fanex.NAP.CTS.Model.AssociatedGroupMonitor
{
    using System;    

    public class AssociatedAccountMonitorModel
    {
        public string Username { get; set; }

        public string AccountStatus { get; set; }

        public DateTime AccountCreated { get; set; }

        public string Site { get; set; }

        public string Currency { get; set; }

        public int Ori { get; set; }

        public int ABI { get; set; }

        public string Category { get; set; }

        public string Remark { get; set; }

        public string DetectedBy { get; set; }

        public DateTime DetectedDate { get; set; }

    }
}
