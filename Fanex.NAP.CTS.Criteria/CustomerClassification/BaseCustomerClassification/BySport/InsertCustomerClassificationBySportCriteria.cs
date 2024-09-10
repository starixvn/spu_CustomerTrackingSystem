namespace Fanex.NAP.CTS.Criteria.CustomerClassification.BaseCustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertCustomerClassificationBySportCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_InputFlowID")]
        public int InputFlowID { get; set; }

        [SpParam(SqlParamName = "ip_CustInfo")]
        public string CustClassificationJson { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_BySport_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
