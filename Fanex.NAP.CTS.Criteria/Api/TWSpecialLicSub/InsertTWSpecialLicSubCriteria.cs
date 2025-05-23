namespace Fanex.NAP.CTS.Criteria.Api.TWSpecialLicSub
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertTWSpecialLicSubCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_Custid")]
        public int Custid { get; set; }

        [SpParam(SqlParamName = "ip_CustomerClass")]
        public int CustomerClass { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_SpecialLicSubCC_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
