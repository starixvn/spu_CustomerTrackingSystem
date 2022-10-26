namespace Fanex.NAP.CTS.Model.AssociationDetection
{
    public class GetCustStatisticsInfoRealtimeModel
    {
        public int CustID { get; set; }

        public decimal TotalWinlossRM { get; set; }

        public decimal WinlossRM_Last3Days { get; set; }

        public decimal WinlossRM_Last7Days { get; set; }

        public decimal TotalTurnoverRM { get; set; }

        public decimal TurnoverRM_Last3Days { get; set; }

        public decimal TurnoverRM_Last7Days { get; set; }

    }
}
