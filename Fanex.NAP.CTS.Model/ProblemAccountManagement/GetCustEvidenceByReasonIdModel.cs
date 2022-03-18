namespace Fanex.NAP.CTS.Model.ProblemAccountManagement
{
    public class GetCustEvidenceByReasonIdModel
    {
        public long CTSCustID { get; set; }

        public long CustID { get; set; }

        public int EvidenceID { get; set; }

        public string EvidenceCode { get; set; }

        public string Remark { get; set; }

    }
}
