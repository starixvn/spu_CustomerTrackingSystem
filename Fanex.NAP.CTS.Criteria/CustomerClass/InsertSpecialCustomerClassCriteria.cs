using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.CustomerClass
{
    public class InsertSpecialCustomerClassCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_Username")]
        public string Username { get; set; }

        [SpParam(SqlParamName = "ip_CustomerClass")]
        public int CustomerClass { get; set; }

        [SpParam(SqlParamName = "ip_IsAffectedToAssociation")]
        public bool IsAffectToAssociation { get; set; }

        [SpParam(SqlParamName = "ip_UserID")]
        public int CreatedBy { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_InsertSpecialCustomerClass";

        public override bool IsValid()
            => true;
    }
}
