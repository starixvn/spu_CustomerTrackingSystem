namespace Fanex.NAP.CTS.Model.Association
{
    using Fanex.NAP.CTS.Model.Transaction;
    using System.Collections.Generic;
    using System.Linq;

    public class StakeIpPatternDetailModel
    {
        public StakeIpPatternDetailModel()
        {
            StakeFraudDetail = Enumerable.Empty<CoupleFraudDetailModel>();
            IpFraudDetail = Enumerable.Empty<CoupleIPFraudDetailModel>();
        }

        public long CustId1 { get; set; }

        public long CustId2 { get; set; }

        public int LastGroupID { get; set; }

        public int LastGroupCustID { get; set; }

        public IEnumerable<TicketDetailsModel> TicketDetails { get; set; }

        public IEnumerable<CoupleFraudDetailModel> StakeFraudDetail { get; set; }

        public IEnumerable<CoupleIPFraudDetailModel> IpFraudDetail { get; set; }
    }
}
