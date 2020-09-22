namespace Fanex.NAP.CTS.Criteria.CTSUser
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertCTSUserCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_userId")]
        public int UserId { get; set; }

        [SpParam(SqlParamName = "ip_userName")]
        public string UserName { get; set; }
      
        public override string GetSettingKey()
            => "CTS_AD_CTSUser_Insert";

        public override bool IsValid()
            => true;
    }
}