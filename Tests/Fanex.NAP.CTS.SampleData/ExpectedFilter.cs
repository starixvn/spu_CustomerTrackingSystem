namespace Fanex.NAP.CTS.SampleData
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using Fanex.NAP.CTS.ViewModel.Account.Evidence;
    using Fanex.NAP.CTS.ViewModel.Account.GeneralInformation;
    using Fanex.NAP.CTS.ViewModel.Account.Search;
    using Fanex.NAP.CTS.ViewModel.Account.TransactionHistory;

    public static class ExpectedFilter
    {
        private static SearchAccountFilter expectedSearchAccountFilter;
        private static GeneralInformationFilter expectedGeneralInformationFilter;
        private static ChainsToEvidenceFilter expectedChainsToEvidenceFilter;
        private static RetractedAndExceptionEvidenceFilter expectedRetractedAndExceptionEvidenceFilter;
        private static AssociatedAccountByAccountFilter expectedAssociatedAccountByAccountFilter;
        private static TransactionHistoryFilter expectedTransactionHistoryFilter;

        public static SearchAccountFilter ExpectedSearchAccountFilter
        {
            get
            {
                expectedSearchAccountFilter = expectedSearchAccountFilter ?? GenerateSampleSearchAccountFilter();

                return expectedSearchAccountFilter;
            }
        }

        public static GeneralInformationFilter ExpectedGeneralInformationFilter
        {
            get
            {
                expectedGeneralInformationFilter = expectedGeneralInformationFilter ?? GenerateSampleGeneralInformationFilter();

                return expectedGeneralInformationFilter;
            }
        }

        public static ChainsToEvidenceFilter ExpectedChainsToEvidenceFilter
        {
            get
            {
                expectedChainsToEvidenceFilter = expectedChainsToEvidenceFilter ?? GenerateSampleChainsToEvidenceFilter();

                return expectedChainsToEvidenceFilter;
            }
        }

        public static RetractedAndExceptionEvidenceFilter ExpectedRetractedAndExceptionEvidenceFilter
        {
            get
            {
                expectedRetractedAndExceptionEvidenceFilter = expectedRetractedAndExceptionEvidenceFilter ?? GenerateSampleRetractedAndExceptionEvidenceFilter();

                return expectedRetractedAndExceptionEvidenceFilter;
            }
        }

        public static AssociatedAccountByAccountFilter ExpectedAssociatedAccountByAccountFilter
        {
            get
            {
                expectedAssociatedAccountByAccountFilter = expectedAssociatedAccountByAccountFilter ?? GenerateSampleAssociatedAccountByAccountFilter();

                return expectedAssociatedAccountByAccountFilter;
            }
        }

        public static TransactionHistoryFilter ExpectedTransactionHistoryFilter
        {
            get
            {
                expectedTransactionHistoryFilter = expectedTransactionHistoryFilter ?? GenerateSampleTransactionHistoryFilter();

                return expectedTransactionHistoryFilter;
            }
        }

        private static SearchAccountFilter GenerateSampleSearchAccountFilter()
            => new SearchAccountFilter
            {
                Key = "12b",
                Subscribers = "-1",
                UserId = 8
            };

        private static GeneralInformationFilter GenerateSampleGeneralInformationFilter()
            => new GeneralInformationFilter
            {
                AccountId = 9,
                CustId = 37778546,
                IsMasterSubscriberAssignment = true,
                UserId = 8
            };

        private static ChainsToEvidenceFilter GenerateSampleChainsToEvidenceFilter()
            => new ChainsToEvidenceFilter
            {
                AccountId = 9,
                CustId = 37778546,
                IsMasterSubscriberAssignment = true,
                UserId = 8
            };

        private static RetractedAndExceptionEvidenceFilter GenerateSampleRetractedAndExceptionEvidenceFilter()
            => new RetractedAndExceptionEvidenceFilter
            {
                AccountId = 9,
                IsMasterSubscriberAssignment = true,
                UserId = 8
            };

        private static AssociatedAccountByAccountFilter GenerateSampleAssociatedAccountByAccountFilter()
            => new AssociatedAccountByAccountFilter
            {
                CustId = 37778546,
                AccountId = 9,
                DeviceAwayMore = false,
                DeviceAwayOne = true,
                EvidenceStatus = EvidenceStatus.All,
                isMasterSubscriberAssignment = true,
                Status = "Open,Suspended,Closed,CN88_Active,CN88_ViewOnly,CN88_Suspended,CN88_Inactive,NotYetDeposit",
                userId = 8
            };

        private static TransactionHistoryFilter GenerateSampleTransactionHistoryFilter()
            => new TransactionHistoryFilter
            {
                AccountId = 9,
                ReportLength = TransactionHistoryLength.Days7
            };
    }
}