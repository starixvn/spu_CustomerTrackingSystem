namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    public class CustInfoModel
    {
        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public int CategoryType { get; set; } // 1: PA, 2: PA Dangerous
    }
}