namespace Fanex.NAP.CTS.Model.AssociatedGroupMonitor
{   
    public class CustInfoAssociatedAccountModel
    {        
        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public int RoleID { get; set; }

        public int SubscriberID { get; set; }

        public bool? IsLicensee { get; set; }

        public int? CategoryID { get; set; }

        public int CategoryGroup { get; set; }

        public bool IsSyncPA { get; set; }
    }
}
