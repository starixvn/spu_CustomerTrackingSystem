namespace Fanex.NAP.CTS.SampleData
{
    using System;
    using Fanex.NAP.CTS.Criteria.CheckBetReport;
    using Fanex.NAP.CTS.Criteria.CustomerMonitor;
    using Fanex.NAP.CTS.Criteria.CustomerMonitor.CustomerDetails;
    using Fanex.NAP.CTS.Criteria.DailyDataCapture;
    using Fanex.NAP.CTS.Criteria.FraudCase.DelayedGoal;

    public static class ExpectedCriteria
    {
        private static CustomerGetCriteria expectedCustomerGetCriteria;
        private static UpdateNoteCriteria expectedUpdateNoteCriteria;
        private static GetCustomerTicketsCriteria expectedGetCustomerTicketsCriteria;
        private static CheckBetReportGetCriteria expectedCheckBetReportGetCriteria;
        private static CheckBetReportUpdateNoteCriteria expectedCheckBetReportUpdateNoteCriteria;       
        private static DailyDataGetCriteria expectedDailyDataGetCriteria;
        private static DelayGoalAssociationAccountInsertCriteria expectedDelayGoalAssociationAccountInsertCriteria;

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

        public static DailyDataGetCriteria ExpectedDailyDataGetCriteria
        {
            get
            {
                expectedDailyDataGetCriteria = expectedDailyDataGetCriteria ?? GenerateSampleDailyDataGetCriteria();

                return expectedDailyDataGetCriteria;
            }
        }

        public static DelayGoalAssociationAccountInsertCriteria ExpectedDelayGoalAssociationAccountInsertCriteria
        {
            get
            {
                expectedDelayGoalAssociationAccountInsertCriteria = expectedDelayGoalAssociationAccountInsertCriteria ?? GenerateSampleDelayGoalAssociationAccountInsertCriteria();

                return expectedDelayGoalAssociationAccountInsertCriteria;
            }
        }    

        private static CustomerGetCriteria GenerateSampleCustomerGetCriteria()
            => new CustomerGetCriteria
            {
                FromDate = new DateTime(2019, 11, 15),
                ToDate = new DateTime(2019, 11, 18),
                Environment = 1,
                IsShowSpecialCurrencies = false
            };        

        private static UpdateNoteCriteria GenerateSampleUpdateNoteCriteria()
            => new UpdateNoteCriteria
            {
                CustId = 37778546,
                UserId = 8,
                Notes = "notes"
            };
        
        private static GetCustomerTicketsCriteria GenerateSampleGetCustomerTicketsCriteria()
            => new GetCustomerTicketsCriteria
            {
                CustId = 37778546,
                FromDate = new DateTime(2019, 11, 15),
                ToDate = new DateTime(2019, 11, 18),
                Environment = 1,
                IsShowSpecialCurrencies = false
            };

        private static CheckBetReportGetCriteria GenerateSampleCheckBetReportGetCriteria()
            => new CheckBetReportGetCriteria
            {
                FromDate = new DateTime(2019, 01, 01),
                ToDate = new DateTime(2019, 11, 18),
                IsShowSpecialCurrencies = false
            };

        private static CheckBetReportUpdateNoteCriteria GenerateSampleCheckBetReportUpdateNoteCriteria()
            => new CheckBetReportUpdateNoteCriteria
            {
                Note = "note",
                TransId = 106484632547,
                UserId = 8
            };        

        private static DailyDataGetCriteria GenerateSampleDailyDataGetCriteria()
            => new DailyDataGetCriteria
            {
                FromDate = new DateTime(2019, 1, 1),
                ToDate = new DateTime(2019, 11, 26)
            };        

        private static DelayGoalAssociationAccountInsertCriteria GenerateSampleDelayGoalAssociationAccountInsertCriteria()
            => new DelayGoalAssociationAccountInsertCriteria
            {
                CustomerAssociationXML = string.Empty
            };                
    }
}