using Fanex.NAP.CTS.Resource.Consts;

namespace Fanex.NAP.CTS.Model.API.ProblemCaseMonitor
{
    public class ProblemCaseMonitorModel
    {
        public long CTSCustID { get; set; }

        public string UserName { get; set; }

        public string SubscriberName { get; set; }

        public int ProblemCaseID { get; set; }

        public string ProblemCaseName { get; set; }

        public bool IsRobot { get; set; }

        public bool IsSuspiciousAssociation { get; set; }

        public bool IsProPunter { get; set; }

        public static ProblemCaseMonitorModel operator +(ProblemCaseMonitorModel info1, ProblemCaseMonitorModel info2)
        {
            if (info1 == null)
            {
                return new ProblemCaseMonitorModel
                {
                    CTSCustID = info2.CTSCustID,
                    UserName = info2.UserName,
                    SubscriberName = info2.SubscriberName,
                    IsRobot = info2.ProblemCaseID == Consts.ProblemCase_ItemID_Robot,                           
                    IsSuspiciousAssociation = info2.ProblemCaseID == Consts.ProblemCase_ItemID_SuspiciousAssociation,
                    IsProPunter = info2.ProblemCaseID == Consts.ProblemCase_ItemID_ProPunter
                };
            }

            if (info2 == null)
            {
                return new ProblemCaseMonitorModel
                {
                    CTSCustID = info1.CTSCustID,
                    UserName = info1.UserName,
                    SubscriberName = info1.SubscriberName,
                    IsRobot = info1.ProblemCaseID == Consts.ProblemCase_ItemID_Robot,
                    IsSuspiciousAssociation = info1.ProblemCaseID == Consts.ProblemCase_ItemID_SuspiciousAssociation,
                    IsProPunter = info1.ProblemCaseID == Consts.ProblemCase_ItemID_ProPunter
                };
            }

            return new ProblemCaseMonitorModel
            {
                CTSCustID = info1.CTSCustID,
                UserName = info1.UserName,
                SubscriberName = info1.SubscriberName,
                IsRobot = info1.ProblemCaseID == Consts.ProblemCase_ItemID_Robot 
                            || info2.ProblemCaseID == Consts.ProblemCase_ItemID_Robot,
                IsSuspiciousAssociation = info1.ProblemCaseID == Consts.ProblemCase_ItemID_SuspiciousAssociation 
                            || info2.ProblemCaseID == Consts.ProblemCase_ItemID_SuspiciousAssociation,
                IsProPunter = info1.ProblemCaseID == Consts.ProblemCase_ItemID_ProPunter
                            || info2.ProblemCaseID == Consts.ProblemCase_ItemID_ProPunter
            };
        }
    }
}
