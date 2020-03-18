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
       
    }
}