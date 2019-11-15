namespace Fanex.NAP.CTS.SampleData
{
    using Fanex.NAP.CTS.Model.Subscriber;

    public static class ExpectedUpdate
    {
        private static SubscriberAssignmentUpdate expectedSubscriberAssignmentUpdate;

        public static SubscriberAssignmentUpdate ExpectedSubscriberAssignmentUpdate
        {
            get
            {
                expectedSubscriberAssignmentUpdate = expectedSubscriberAssignmentUpdate ?? GenerateSampleSubscriberAssignmentUpdate();

                return expectedSubscriberAssignmentUpdate;
            }
        }

        public static SubscriberAssignmentUpdate GenerateSampleSubscriberAssignmentUpdate()
        {
            return new SubscriberAssignmentUpdate
            {
            };
        }
    }
}