namespace Fanex.NAP.CTS.SampleData
{
    using Fanex.NAP.CTS.ViewModel.Account.GeneralInformation;
    using Fanex.NAP.CTS.ViewModel.Account.Search;

    public static class ExpectedFilter
    {
        private static SearchAccountFilter expectedSearchAccountFilter;
        private static GeneralInformationFilter expectedGeneralInformationFilter;

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
    }
}
