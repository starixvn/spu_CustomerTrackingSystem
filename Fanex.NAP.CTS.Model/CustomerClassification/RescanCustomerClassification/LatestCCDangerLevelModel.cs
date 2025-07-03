namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    public class LatestCCDangerLevelModel
    {
        public long CustID { get; set; }

        public int CategoryID { get; set; }

        public int CustomerClass { get; set; }

        public string CustomerClassName { get; set; }

        public int? DangerLevel { get; set; } // ABI

        public int? Danger1 { get; set; }  // Ori
    }
}