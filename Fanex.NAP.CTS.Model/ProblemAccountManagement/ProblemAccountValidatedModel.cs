namespace Fanex.NAP.CTS.Model.ProblemAccountManagement
{
    public class ProblemAccountValidatedModel
    {
        public string UserName { get; set; }

        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public string Site { get; set; }

        public int RoleID { get; set; }

        public int ReasonID { get; set; }

        public int ActionID { get; set; }

        public string Comment { get; set; }

        public string ErrorMsg { get; set; }
    }
}
