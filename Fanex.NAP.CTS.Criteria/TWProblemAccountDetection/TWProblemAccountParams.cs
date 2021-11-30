namespace Fanex.NAP.CTS.Criteria.TWProblemAccountDetection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TWProblemAccountParams
    {
        public string CustomerList { get; set; }

        public int RoleId { get; set; }

        public int ReasonId { get; set; }

        public int PreventActionId { get; set; }

        public string Comment { get; set; }

        public string VoidTime { get; set; }

        public decimal LimitAmount { get; set; }

        public int UserId { get; set; }

        public string IP { get; set; }
    }
}
