using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.Account.CustomerCategory
{
    public class GetAllSportGroupCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_DC_Common_GetAllSportGroups";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
