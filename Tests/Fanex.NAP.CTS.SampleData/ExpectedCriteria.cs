namespace Fanex.NAP.CTS.SampleData
{
    using System;
    using Fanex.NAP.CTS.Criteria.CheckBetReport;
    using Fanex.NAP.CTS.Criteria.CustomerMonitor;
    using Fanex.NAP.CTS.Criteria.CustomerMonitor.CustomerDetails;

    public static class ExpectedCriteria
    {
        private static CustomerGetCriteria expectedCustomerGetCriteria;
        private static UpdateNoteCriteria expectedUpdateNoteCriteria;
        private static GetCustomerTicketsCriteria expectedGetCustomerTicketsCriteria;
        private static CheckBetReportGetCriteria expectedCheckBetReportGetCriteria;
        private static CheckBetReportUpdateNoteCriteria expectedCheckBetReportUpdateNoteCriteria;

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

        public static CheckBetReportGetCriteria ExpectedCheckBetReportGetCriteria
        {
            get
            {
                expectedCheckBetReportGetCriteria = expectedCheckBetReportGetCriteria ?? GenerateSampleCheckBetReportGetCriteria();

                return expectedCheckBetReportGetCriteria;
            }
        }

        public static CheckBetReportUpdateNoteCriteria ExpectedCheckBetReportUpdateNoteCriteria
        {
            get
            {
                expectedCheckBetReportUpdateNoteCriteria = expectedCheckBetReportUpdateNoteCriteria ?? GenerateSampleCheckBetReportUpdateNoteCriteria();

                return expectedCheckBetReportUpdateNoteCriteria;
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

        private static CheckBetReportGetCriteria GenerateSampleCheckBetReportGetCriteria()
        {
            return new CheckBetReportGetCriteria
            {
                FromDate = new DateTime(2019, 01, 01),
                ToDate = new DateTime(2019, 11, 18),
                IsShowSpecialCurrencies = false
            };
        }

        private static CheckBetReportUpdateNoteCriteria GenerateSampleCheckBetReportUpdateNoteCriteria()
        {
            return new CheckBetReportUpdateNoteCriteria
            {
                Note = "note",
                TransId = 106484632547,
                UserId = 8
            };
        }
    }
}