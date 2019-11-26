namespace Fanex.NAP.CTS.SampleData
{
    using Fanex.NAP.CTS.Model.Subscriber;
    using Fanex.NAP.CTS.ViewModel.Account.Evidence.Exception;

    public static class ExpectedUpdate
    {
        private static SubscriberAssignmentUpdate expectedSubscriberAssignmentUpdate;
        private static ExceptionEvidenceAdd expectedExceptionEvidenceAdd;

        public static SubscriberAssignmentUpdate ExpectedSubscriberAssignmentUpdate
        {
            get
            {
                expectedSubscriberAssignmentUpdate = expectedSubscriberAssignmentUpdate ?? GenerateSampleSubscriberAssignmentUpdate();

                return expectedSubscriberAssignmentUpdate;
            }
        }

        public static ExceptionEvidenceAdd ExpectedExceptionEvidenceAdd
        {
            get
            {
                expectedExceptionEvidenceAdd = expectedExceptionEvidenceAdd ?? GenerateSampleExceptionEvidenceAdd();

                return expectedExceptionEvidenceAdd;
            }
        }

        private static SubscriberAssignmentUpdate GenerateSampleSubscriberAssignmentUpdate()
            => new SubscriberAssignmentUpdate { };

        private static ExceptionEvidenceAdd GenerateSampleExceptionEvidenceAdd()
            => new ExceptionEvidenceAdd
            {
                AccountId = 9,
                ExceptionAccountId = 1,
                SubscriberId = 6,
                Comment = "comment",
                UserId = 8,
            };      
    }
}