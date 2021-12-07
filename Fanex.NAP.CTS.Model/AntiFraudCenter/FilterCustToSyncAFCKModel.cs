namespace Fanex.NAP.CTS.Model.API
{
    using System.Collections.Generic;
    using System.Linq;

    public class FilterCustToSyncAFCKModel
    {
        public int CustID { get; set; }

        public string FraudID { get; set; }

        public IEnumerable<int> FraudIDList
        {
            get
            {
                if (!string.IsNullOrEmpty(FraudID))
                {
                    return FraudID.Split(',').Select(int.Parse);
                }

                return new List<int>();
            }
        }
    }
}