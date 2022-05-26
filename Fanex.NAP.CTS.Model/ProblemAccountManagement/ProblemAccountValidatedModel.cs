namespace Fanex.NAP.CTS.Model.ProblemAccountManagement
{
    public class ProblemAccountValidatedModel
    {
        public long CTSCustID { get; set; }

        public int SubscriberID { get; set; }

        public int RoleID { get; set; }

        public long CustID { get; set; }

        public string UserName { get; set; }

        public bool IsLicensee { get; set; }

        public int ActionID { get; set; }        

        public string ErrorMsg { get; set; }
    }
}
