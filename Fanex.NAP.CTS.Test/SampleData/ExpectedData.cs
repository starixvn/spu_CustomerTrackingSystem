namespace Fanex.NAP.CTS.Test.SampleData
{
    using Fanex.NAP.CTS.ViewModel.Account.Search;
    using System.Collections.Generic;

    public static class ExpectedData
    {
        private static List<SubscriberAccount> expectedSubscriberAccounts;

        public static List<SubscriberAccount> ExpectedSubscriberAccounts
        {
            get
            {
                expectedSubscriberAccounts = expectedSubscriberAccounts ?? GenerateSubscriberAccounts();

                return expectedSubscriberAccounts;
            }
        }

        public static List<SubscriberAccount> GenerateSubscriberAccounts()
        {
            return new List<SubscriberAccount>();
        }
    }
}
