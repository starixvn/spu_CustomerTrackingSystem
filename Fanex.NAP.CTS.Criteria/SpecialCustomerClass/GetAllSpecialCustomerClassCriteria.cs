namespace Fanex.NAP.CTS.Criteria.SpecialCustomerClass
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAllSpecialCustomerClassCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_IsGetAllInfo")]
        public bool IsGetAllInfo { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_SpecialCustClass_GetAll";

        public override bool IsValid()
            => true;
    }
}
