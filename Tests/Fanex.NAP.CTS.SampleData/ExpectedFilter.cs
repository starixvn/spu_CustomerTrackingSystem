namespace Fanex.NAP.CTS.SampleData
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using Fanex.NAP.CTS.ViewModel.Account.GeneralInformation;
    using Fanex.NAP.CTS.ViewModel.Account.Search;
    using Fanex.NAP.CTS.ViewModel.Account.TransactionHistory;

    public static class ExpectedFilter
    {
        private static SearchAccountFilter expectedSearchAccountFilter;
        private static GeneralInformationFilter expectedGeneralInformationFilter;
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

        public static SearchAccountFilter GenerateSampleSearchAccountFilter()
        {
            return new SearchAccountFilter
            {
                Key = "12b",
                Subscribers = "-1",
                UserId = 8
            };
        }

        public static GeneralInformationFilter GenerateSampleGeneralInformationFilter()
        {
            return new GeneralInformationFilter
            {
            };
        }

        public static AssociatedAccountByAccountFilter GenerateSampleAssociatedAccountByAccountFilter()
        {
            return new AssociatedAccountByAccountFilter
            {
            };
        }

        public static TransactionHistoryFilter GenerateSampleTransactionHistoryFilter()
        {
            return new TransactionHistoryFilter
            {
                AccountId = 12345,
                ReportLength = TransactionHistoryLength.Hours24
            };
        }
    }
}