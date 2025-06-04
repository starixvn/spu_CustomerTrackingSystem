namespace Fanex.NAP.CTS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetRoleCustomerCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustIDs", AllowNull = true)]
        public string CustIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_GetCustIDsRole";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
