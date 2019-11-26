namespace Fanex.NAP.CTS.SampleData
{
    using Fanex.NAP.CTS.Model.Subscriber;
    using Fanex.NAP.CTS.ViewModel.Account;
    using Fanex.NAP.CTS.ViewModel.CustomerMonitor.CustomerDetails;
    using System;
    using System.Collections.Generic;

    public static class ExpectedData
    {
        private static List<SubscriberAssignment> expectedSubscriberAssignments;
        private static AccountDetail expectedAccountDetail;
        private static CustomerDetailsInfo expectedCustomerDetailsInfo;

        public static List<SubscriberAssignment> ExpectedSubscriberAssignments
        {
            get
            {
                expectedSubscriberAssignments = expectedSubscriberAssignments ?? GenerateSampleSubscriberAssignments();

                return expectedSubscriberAssignments;
            }
        }

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

        private static List<SubscriberAssignment> GenerateSampleSubscriberAssignments()
            => new List<SubscriberAssignment>
            {
                new SubscriberAssignment
                {
                    SubscriberID = 1,
                    SubscriberName = "11Bet",
                    isAssigned = 0,
                    IsMaster = 1
                },
                new SubscriberAssignment
                {
                    SubscriberID = 2,
                    SubscriberName = "12Bet",
                    isAssigned = 1,
                    IsMaster = 1
                }
            };

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