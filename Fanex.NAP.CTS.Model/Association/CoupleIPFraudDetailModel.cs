namespace Fanex.NAP.CTS.Model.Association
{
    using System;

    public class CoupleIPFraudDetailModel: FraudDetailModel
    {
        public int TypeIPEntanglement { get; set; }
        public IPPatternType3S IPPatternType
        {
            get
            {
                IPPatternType3S result;
                Enum.TryParse(TypeIPEntanglement.ToString(), out result);                
                return result;
            }
        }
    }
}
