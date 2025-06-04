namespace Fanex.NAP.CTS.Criteria.CustomerClassification.BaseCustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RemoveCustomerClassificationBySportCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_InputFlowID", AllowNull = true)]
        public int InputFlowID { get; set; }

        [SpParam(SqlParamName = "ip_CustInfo", AllowNull = true)]
        public string CustClassificationJson { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage", AllowNull = true)]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_BySport_Remove";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
