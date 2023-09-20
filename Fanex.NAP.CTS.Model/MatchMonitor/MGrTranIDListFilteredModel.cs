namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    public class MGrTranIDListFilteredModel
    {
        public string BetID { get; set; }
       
        public string MGrName { get; set; }

        public long TransID
        {
            get
            {
                if (string.IsNullOrEmpty(this.BetID))
                {
                    return 0;
                }
                else
                {
                    long.TryParse(this.BetID, out long transID);

                    return transID;
                }
            }
        }

        public int MGr 
        { 
            get 
            {
                if (string.IsNullOrEmpty(this.MGrName) || this.MGrName == "-")
                {
                    return 0;
                }
                else
                {
                    int.TryParse(this.MGrName, out int mGr);

                    return mGr;
                }
            } 
        }
    }
}
