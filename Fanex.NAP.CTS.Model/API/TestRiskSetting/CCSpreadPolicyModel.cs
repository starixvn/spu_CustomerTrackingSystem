using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.API.TestRiskSetting
{
    public class CCSpreadPolicyModel
    {
        public CCSpreadPolicyModel()
        {
            SpreadPolicy = new List<CCSpreadPolicyEntity>();
            SpreadPolicyDetail = new List<CCSpreadPolicyDetailEntity>();
        }

        public IEnumerable<CCSpreadPolicyEntity> SpreadPolicy { get; set; }

        public IEnumerable<CCSpreadPolicyDetailEntity> SpreadPolicyDetail { get; set; }
    }
}
