namespace Fanex.NAP.CTS.SampleData
{
    using System;
    using Fanex.NAP.CTS.Criteria.CustomerMonitor;
    using Fanex.NAP.CTS.Criteria.CustomerMonitor.CustomerDetails;

    public static class ExpectedCriteria
    {
        private static CustomerGetCriteria expectedCustomerGetCriteria;
        private static UpdateNoteCriteria expectedUpdateNoteCriteria;
        private static GetCustomerTicketsCriteria expectedGetCustomerTicketsCriteria;

        public static CustomerGetCriteria ExpectedCustomerGetCriteria
        {
            get
            {
                expectedCustomerGetCriteria = expectedCustomerGetCriteria ?? GenerateSampleCustomerGetCriteria();

                return expectedCustomerGetCriteria;
            }
        }

        public static UpdateNoteCriteria ExpectedUpdateNoteCriteria
        {
            get
            {
                expectedUpdateNoteCriteria = expectedUpdateNoteCriteria ?? GenerateSampleUpdateNoteCriteria();

                return expectedUpdateNoteCriteria;
            }
        }

        public static GetCustomerTicketsCriteria ExpectedGetCustomerTicketsCriteria
        {
            get
            {
                expectedGetCustomerTicketsCriteria = expectedGetCustomerTicketsCriteria ?? GenerateSampleGetCustomerTicketsCriteria();

                return expectedGetCustomerTicketsCriteria;
            }
        }

        private static CustomerGetCriteria GenerateSampleCustomerGetCriteria()
        {
            return new CustomerGetCriteria
            {
                FromDate = new DateTime(2019, 11, 15),
                ToDate = new DateTime(2019, 11, 18),
                Environment = 1,
                IsShowSpecialCurrencies = false,
            };
        }

        private static UpdateNoteCriteria GenerateSampleUpdateNoteCriteria()
        {
            return new UpdateNoteCriteria
            {
                CustId = 37778546,
                UserId = 8,
                Notes = "notes"
            };
        }

        private static GetCustomerTicketsCriteria GenerateSampleGetCustomerTicketsCriteria()
        {
            return new GetCustomerTicketsCriteria
            {
                CustId = 37778546,
                FromDate = new DateTime(2019, 11, 15),
                ToDate = new DateTime(2019, 11, 18),
                Environment = 1,
                IsShowSpecialCurrencies = false
            };
        }
    }
}