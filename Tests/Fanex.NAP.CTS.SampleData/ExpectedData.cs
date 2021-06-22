namespace Fanex.NAP.CTS.SampleData
{
    using Fanex.NAP.CTS.ViewModel.Account;
    using Fanex.NAP.CTS.ViewModel.CustomerMonitor.CustomerDetails;
    using System;
    using System.Collections.Generic;

    public static class ExpectedData
    {        
        private static AccountDetail expectedAccountDetail;
        private static CustomerDetailsInfo expectedCustomerDetailsInfo;
    
        public static AccountDetail ExpectedAccountDetail
        {
            get
            {
                expectedAccountDetail = expectedAccountDetail ?? GenerateSampleAccountDetail();

                return expectedAccountDetail;
            }
        }

        public static CustomerDetailsInfo ExpectedCustomerDetailsInfo
        {
            get
            {
                expectedCustomerDetailsInfo = expectedCustomerDetailsInfo ?? GenerateSampleCustomerDetailsInfo();

                return expectedCustomerDetailsInfo;
            }
        }        

        private static AccountDetail GenerateSampleAccountDetail()
            => new AccountDetail { };

        private static CustomerDetailsInfo GenerateSampleCustomerDetailsInfo()
            => new CustomerDetailsInfo
            {
                CustId = 37778546,
                FromDate = new DateTime(2019, 11, 15),
                ToDate = new DateTime(2019, 11, 18),
                Username = "sa",
                MatchCount = 1,
                TicketCount = 1,
                ViolationCount = 1
            };
    }
}