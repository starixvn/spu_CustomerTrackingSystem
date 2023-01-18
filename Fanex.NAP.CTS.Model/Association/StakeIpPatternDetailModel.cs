namespace Fanex.NAP.CTS.Model.Association
{
    using Fanex.NAP.CTS.Model.Transaction;
    using System.Collections.Generic;

    public class StakeIpPatternDetailModel
    {
        public long CoupleID { get; set; }

        public int LastGroupID { get; set; }

        public int LastGroupCustID { get; set; }

        public IEnumerable<TicketDetailsModel> TicketDetailsModels { get; set; }
        
        public IEnumerable<CoupleFraudDetailModel> CoupleFraudDetails { get; set; }

        public IEnumerable<CoupleIPFraudDetailModel> CoupleIPFraudDetails { get; set; }
    }
}
