namespace Fanex.NAP.CTS.Criteria.CustomerClassificationAgency.BaseCustomerClassificationAgency
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RemoveCustomerClassificationAgencyCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_InputFlowID", AllowNull = true)]
        public int InputFlowID { get; set; }

        [SpParam(SqlParamName = "ip_FromAction", AllowNull = true)]
        public int FromAction { get; set; }

        [SpParam(SqlParamName = "ip_UplineRoleID", AllowNull = true)]
        public int UplineRoleID { get; set; }

        [SpParam(SqlParamName = "ip_QueueID", AllowNull = true)]
        public long? QueueID { get; set; }

        [SpParam(SqlParamName = "ip_IsAffectDownline", AllowNull = true)]
        public bool? IsAffectDownline { get; set; }

        [SpParam(SqlParamName = "ip_IsFromQueue", AllowNull = true)]
        public bool IsFromQueue { get; set; }

        [SpParam(SqlParamName = "ip_CustInfo", AllowNull = true)]
        public string CustClassificationJson { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage", AllowNull = true)]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassificationAgency_Remove"; // new
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
