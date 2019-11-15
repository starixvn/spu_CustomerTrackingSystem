namespace Fanex.NAP.CTS.SampleData
{
    using Fanex.NAP.CTS.Model.Subscriber;
    using Fanex.NAP.CTS.ViewModel.Account;
    using System.Collections.Generic;

    public static class ExpectedData
    {
        private static List<SubscriberAssignment> expectedSubscriberAssignments;

        private static AccountDetail expectedAccountDetail;

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

        public static List<SubscriberAssignment> GenerateSampleSubscriberAssignments()
        {
            return new List<SubscriberAssignment>
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
        }

        public static AccountDetail GenerateSampleAccountDetail()
        {
            return new AccountDetail
            {
            };
        }
    }
}
