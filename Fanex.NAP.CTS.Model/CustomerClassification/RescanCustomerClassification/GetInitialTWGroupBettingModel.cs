namespace Fanex.NAP.CTS.Model.CustomerClassification.RescanCustomerClassification
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Resource.Consts;
    using System;

    public class GetInitialTWGroupBettingModel
    {
        public long CustID { get; set; }

        public DateTime DetectedDate { get; set; }

        public int? GBTicketCount { get; set; }

        public int? SportType { get; set; }

        public string SportName
        {
            get
            {
                if (this.SportType != null)
                {
                    return CachedRef.GetSportTypeName(this.SportType.Value);
                }

                return Consts.TWInitialGBMissSport;
            }
        }
    }
}
