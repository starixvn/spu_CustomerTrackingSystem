namespace Fanex.NAP.CTS.Model.CustomerClassificationAgency.BaseCustomerClassificationAgency
{
    using System;

    public class RescanCustomerClassificationAgencyModel
    {
        public long CTSCustID { get; set; }

        public long CustID { get; set; }

        public int CategoryID { get; set; }
     
        public int? SportType { get; set; }

        public int? IssueTypeID { get; set; }

        public int RoleID { get; set; }

        public int SubscriberID { get; set; }

        public bool IsLicensee { get; set; }

        public string Remark { get; set; }

        public long CreatedBy { get; set; }

        public bool IsMarkedDirectly { get; set; }

        public bool IsFromCTS { get; set; }

        public bool IsFromTW { get; set; }

        public int? RobotCounter { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public int InputFlowID { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
