namespace Fanex.NAP.CTS.Criteria.SpecialCustomerClass
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAllSpecialCustomerClassCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_IsGetAllInfo", AllowNull = true)]
        public bool IsGetAllInfo { get; set; }

        [SpParam(SqlParamName = "ip_Username", AllowNull = true)]
        public string Username { get; set; }

        [SpParam(SqlParamName = "ip_SportList", AllowNull = true)]
        public string SportList { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_SpecialCustClass_GetAll";

        public override bool IsValid()
            => true;
    }
}
