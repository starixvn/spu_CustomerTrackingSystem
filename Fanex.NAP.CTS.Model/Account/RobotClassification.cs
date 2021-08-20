namespace Fanex.NAP.CTS.Model.Account
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RobotClassification
    {
        public long CustID { get; set; }

        public int SportTypeID { get; set; }

        public int BetTypeID { get; set; }

        public bool LiveIndicator { get; set; }

        public int RobotType { get; set; }

        public DateTime ScannedDate { get; set; }
    }
}
