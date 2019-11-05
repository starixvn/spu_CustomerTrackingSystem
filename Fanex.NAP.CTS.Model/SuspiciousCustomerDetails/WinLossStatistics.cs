namespace Fanex.NAP.CTS.Model.SuspiciousCustomerDetails
{
    public class WinLossStatistics
    {
        public double SumTO { get; set; }

        public double SumWL { get; set; }

        public double Margin => SumTO == 0 ? 0 : SumWL / SumTO * 100;
    }
}