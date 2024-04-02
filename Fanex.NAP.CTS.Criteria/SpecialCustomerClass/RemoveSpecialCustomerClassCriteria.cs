namespace Fanex.NAP.CTS.Criteria.SpecialCustomerClass
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RemoveSpecialCustomerClassCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ListCust")]
        public string ListCust { get; set; }      

        [SpParam(SqlParamName = "ip_Remark")]
        public string Comment { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy")]
        public int CreatedBy { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_SpecialCustClass_Remove";

        public override bool IsValid()
            => true;
    }
}
