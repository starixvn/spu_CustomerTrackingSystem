namespace Fanex.NAP.CTS.Model.Association
{

    public class CoupleFraudDetailModel: FraudDetailModel
    {
        public long StateID { get; set; }

        public decimal SumStake { get; set; }

        public int BetCount { get; set; }
    }
}
