using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.CustomerClass
{
    public class GetAllSpecialCustomerClassCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_IsGetAllInfo")]
        public bool IsGetAllInfo { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_GetAllSpecialCustomerClass";

        public override bool IsValid()
            => true;
    }
}
