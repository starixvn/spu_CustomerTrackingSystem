namespace Fanex.NAP.CTS.Model.Association
{
    using System;
    using System.Collections.Generic;

    public class CoupleIPFraudDetailModel
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

        public long MatchID { get; set; }

        public DateTime EventDate { get; set; }

        public string FraudTransList { get; set; }

        public IEnumerable<string> FraudTransListSperation
        {
            get
            {
                if (string.IsNullOrEmpty(FraudTransList))
                {
                    return new List<string>();
                }
                char[] sperator = new char[] { ',' };
                return FraudTransList.Split(sperator, StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }
}
