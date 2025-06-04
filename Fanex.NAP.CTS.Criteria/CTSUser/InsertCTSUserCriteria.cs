namespace Fanex.NAP.CTS.Criteria.CTSUser
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertCTSUserCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_userId", AllowNull = true)]
        public int UserId { get; set; }

        [SpParam(SqlParamName = "ip_userName", AllowNull = true)]
        public string UserName { get; set; }
      
        public override string GetSettingKey()
            => "CTS_AD_CTSUser_Insert";

        public override bool IsValid()
            => true;
    }
}
