namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetAssociationProfileCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_AssociationType")]
        public int AssociationType { get; set; }

        [SpParam(SqlParamName = "ip_AssociationStatus")]
        public int AssociationStatus { get; set; }

        [SpParam(SqlParamName = "ip_AccountStatusIDs")]
        public string AccountStatusIds { get; set; }

        [SpParam(SqlParamName = "ip_SiteIDs")]
        public string SiteIds { get; set; }

        [SpParam(SqlParamName = "ip_CategoryIDs")]
        public string CategoryIDs { get; set; }

        [SpParam(SqlParamName = "ip_Skip")]
        public int Skip { get; set; }

        [SpParam(SqlParamName = "ip_Take")]
        public int Take { get; set; }

        [SpParam(SqlParamName = "op_TotalItems")]
        public int TotalItems { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_Profile_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
