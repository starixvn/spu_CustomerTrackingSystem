namespace Fanex.NAP.CTS.Criteria.SuspiciousCustomerDetails
{
    using Fanex.Data.Repository;
    using System;

    public abstract class SuspiciousCustomerDetailsGeneralCriteria: CriteriaBase
    {
        public int CustId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public bool IsShowSpecialCurrencies { get; set; }

        public override bool IsValid() => true;
    }
}