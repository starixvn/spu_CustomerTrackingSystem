namespace Fanex.NAP.CTS.Model.API
{
    public class CustInfoBySiteNameModel
    {
        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public string UserName { get; set; }

        public int RoleID { get; set; }

        public int SiteID { get; set; }

        public bool IsLicensee { get; set; }

        public bool IsLicenseeVIP { get; set; }

        public bool IsLicenseeBA { get; set; }
    }
}