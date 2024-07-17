namespace Fanex.NAP.CTS.Model.API.TWInitialSmart
{
    using System;

    public class InitialSmartCustInfoModel
    {
        public long CustID { get; set; }

        public decimal Probability { get; set; }

        public DateTime PredictTime { get; set; }
    }
}
