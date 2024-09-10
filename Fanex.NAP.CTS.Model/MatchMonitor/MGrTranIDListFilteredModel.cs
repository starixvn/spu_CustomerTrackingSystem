namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    public class MGrTranIDListFilteredModel
    {
        public TicketType TicketType { get; set; }

        public string BetID { get; set; }

        public long Refno { get; set; }

        public string MGrName { get; set; }

        public long TransRefnoID
        {
            get
            {
                if (this.TicketType == TicketType.Single)
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
                else
                {
                    return this.Refno;
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
