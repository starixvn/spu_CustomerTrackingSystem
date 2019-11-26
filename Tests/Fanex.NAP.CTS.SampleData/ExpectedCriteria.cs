namespace Fanex.NAP.CTS.SampleData
{
    using System;
    using Fanex.NAP.CTS.Criteria.CheckBetReport;
    using Fanex.NAP.CTS.Criteria.CustomerMonitor;
    using Fanex.NAP.CTS.Criteria.CustomerMonitor.CustomerDetails;
    using Fanex.NAP.CTS.Criteria.DailyDataCapture;
    using Fanex.NAP.CTS.Criteria.SuspiciousCustomer;

    public static class ExpectedCriteria
    {
        private static CustomerGetCriteria expectedCustomerGetCriteria;
        private static UpdateNoteCriteria expectedUpdateNoteCriteria;
        private static GetCustomerTicketsCriteria expectedGetCustomerTicketsCriteria;
        private static CheckBetReportGetCriteria expectedCheckBetReportGetCriteria;
        private static CheckBetReportUpdateNoteCriteria expectedCheckBetReportUpdateNoteCriteria;
        private static SuspiciousCustomerReportGetCriteria expectedSuspiciousCustomerReportGetCriteria;
        private static DataTableAjaxPostCriteria expectedDataTableAjaxPostCriteria;
        private static DailyDataGetCriteria expectedDailyDataGetCriteria;

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

        public static SuspiciousCustomerReportGetCriteria ExpectedSuspiciousCustomerReportGetCriteria
        {
            get
            {
                expectedSuspiciousCustomerReportGetCriteria = expectedSuspiciousCustomerReportGetCriteria ?? GenerateSampleSuspiciousCustomerReportGetCriteria();

                return expectedSuspiciousCustomerReportGetCriteria;
            }
        }

        public static DataTableAjaxPostCriteria ExpectedDataTableAjaxPostCriteria
        {
            get
            {
                expectedDataTableAjaxPostCriteria = expectedDataTableAjaxPostCriteria ?? GenerateSampleDataTableAjaxPostCriteria();

                return expectedDataTableAjaxPostCriteria;
            }
        }

        public static DailyDataGetCriteria ExpectedDailyDataGetCriteria
        {
            get
            {
                expectedDailyDataGetCriteria = expectedDailyDataGetCriteria ?? GenerateSampleDailyDataGetCriteria();

                return expectedDailyDataGetCriteria;
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

        private static SuspiciousCustomerReportGetCriteria GenerateSampleSuspiciousCustomerReportGetCriteria()
        {
            return new SuspiciousCustomerReportGetCriteria
            {
                AssigneeId = null,
                Classification = 0,
                FraudCaseId = 0,
                FromDate = new DateTime(2019, 11, 1),
                ToDate = new DateTime(2019, 11, 22),
                IsVerified = null,
                IsExportExcel = false,
                IsShowSpecialCurrencies = false,
                PageIndex = 1,
                PageSize = 50,
                TotalItems = 0,
                Username = null
            };
        }

        private static DataTableAjaxPostCriteria GenerateSampleDataTableAjaxPostCriteria()
        {
            return new DataTableAjaxPostCriteria
            {
                AssigneeId = null,
                Classification = 0,
                FraudCaseId = 0,
                FromDate = new DateTime(2019, 11, 1),
                ToDate = new DateTime(2019, 11, 22),
                IsVerified = null,
                IsExportExcel = false,
                IsShowSpecialCurrencies = false,
                PageIndex = 1,
                PageSize = 50,
                TotalItems = 0,
                Username = null,
                length = 50,
                start = 1
            };
        }

        private static DailyDataGetCriteria GenerateSampleDailyDataGetCriteria()
        {
            return new DailyDataGetCriteria
            {
                FromDate = new DateTime(2019, 1, 1),
                ToDate = new DateTime(2019, 11, 26)
            };
        }
    }
}