namespace Fanex.NAP.CTS.Model.SpecialCustomerClass
{

    public class AddUserListSpecialCCModel
    {
        public long CTSCustID { get; set; }

        public long CustID { get; set; }

        public int SportID { get; set; }

        public int SubscriberID { get; set; }

        public bool IsLicenseeVIP { get; set; }
    }

    public class RemoveUserListSpecialCCModel
    {
        public long CTSCustID { get; set; }

        public long CustID { get; set; }

        public int SportID { get; set; }

        public int SubscriberID { get; set; }
    }
}
