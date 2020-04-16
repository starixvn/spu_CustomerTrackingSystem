using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.Home
{
    public class SearchResultCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_username")]
        public string Username { get; set; }

        [SpParam(SqlParamName = "ip_original_username")]
        public string OriginalUserName { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_GetUserInfomationByUsername";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
