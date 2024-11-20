namespace Fanex.NAP.CTS.Model.CustomerClassificationAgency.ProblemAccountAgency
{
    using System;

    public class UnmarkProblemAccountAgencyModel
    {
        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public int RoleID { get; set; }

        public int SubscriberID { get; set; }

        public Int64? CreatedBy { get; set; }

        public string Remark { get; set; }

        public bool IsExceptDirectDownline { get; set; }

        public bool IsMarkedDirectly { get; set; }
    }
}
