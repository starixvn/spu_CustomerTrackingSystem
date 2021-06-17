namespace Fanex.NAP.CTS.Model.ProblemCaseMonitor
{
    using System;

    public class ProblemCasesMonitorModel
    {
        public Int64 CTSCustId { get; set; }

        public string UserName { get; set; }

        public string SubscriberName { get; set; }

        public bool IsRobot { get; set; }

        public bool IsSuspiciousAssociation { get; set; }

        public bool IsProPunter { get; set; }

        public string RobotToString 
        {
            get
            {
                return IsRobot ? "Yes" : "";
            }
        }

        public string SuspiciousAssociationToString
        {
            get
            {
                return IsSuspiciousAssociation ? "Yes" : "";
            }
        }

        public string ProPunterToString
        {
            get
            {
                return IsProPunter ? "Yes" : "";
            }
        }
    }
}
