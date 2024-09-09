namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    public class MMDetailMergeGroupModel
    {
        public long TransID { get; set; }
     
        public long Refno { get { return this.TransID; } }

        public int ScoreDiff { get; set; }

        public string ReasonList { get; set; }

        public int MergeGroupID { get; set; }

    }
}
