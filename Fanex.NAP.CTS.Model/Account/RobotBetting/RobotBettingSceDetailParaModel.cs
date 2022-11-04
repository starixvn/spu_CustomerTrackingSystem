namespace Fanex.NAP.CTS.Model.Account.RobotBetting
{
    using System;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class RobotBettingSceDetailParaModel
    {
        public long CustId { get; set; }

        public int RobotTypeOrigin { get; set; }

        public int RobotTypeDisplay { get; set; }

        public int? ClusterId { get; set; }
        
        public DateTime? ScannedTime { get; set; }
        
        public RobotDetectionQueryType QueryType { get; set; }
    }
}