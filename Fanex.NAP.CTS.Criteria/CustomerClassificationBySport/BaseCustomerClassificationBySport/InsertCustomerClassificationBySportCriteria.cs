namespace Fanex.NAP.CTS.Criteria.CustomerClassificationBySport.BaseCustomerClassificationBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertCustomerClassificationBySportCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_InputFlowID", AllowNull = true)]
        public int InputFlowID { get; set; }

        [SpParam(SqlParamName = "ip_CustInfo", AllowNull = true)]
        public string CustClassificationJson { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage", AllowNull = true)]
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
