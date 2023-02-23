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
                if (Enum.TryParse(TypeIPEntanglement.ToString(), out IPPatternType3S result))
                {
                    return result;
                }               
                return IPPatternType3S.None;
            }
        }
    }
}
