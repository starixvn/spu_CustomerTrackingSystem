namespace Fanex.NAP.CTS.Criteria.Common
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCommonStaticListCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ListID")]
        public int ListID { get; set; }

        [SpParam(SqlParamName = "ip_Status")]
        public bool IsStatusActive { get; set; }
       
        public override string GetSettingKey()
                => "CTS_DC_Common_GetStaticList";

        public override bool IsValid()
            => true;
    }
}
