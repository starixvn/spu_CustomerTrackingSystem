using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;

namespace Fanex.NAP.CTS.Criteria.ProblemAccountMonitor
{
    public class DetailCustomerGroupsGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustomerTransDateList")]
        public string CustomerTransDateList { get; set; }

        public override string GetSettingKey()
           => "CTS_DC_GetCustomerGroup";

        public override bool IsValid()
           => true;
    }
}
