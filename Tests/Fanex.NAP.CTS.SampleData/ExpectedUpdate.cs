namespace Fanex.NAP.CTS.SampleData
{
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.Subscriber;
    using Fanex.NAP.CTS.ViewModel.Account.Evidence.Exception;

    public static class ExpectedUpdate
    {
        private static SubscriberAssignmentUpdate expectedSubscriberAssignmentUpdate;
        private static ExceptionEvidenceAdd expectedExceptionEvidenceAdd;
        private static UpdateAccountExceptionModel expectedUpdateAccountExceptionModel;
        private static UpdatePlaceEvidenceModel expectedUpdatePlaceEvidenceModel;
        private static RemoveEvidenceModel expectedRemoveFlaggedEvidenceModel;
        private static RemoveEvidenceModel expectedRetractAffectedEvidenceModel;
        private static RemoveEvidenceModel expectedRestoreAffectedEvidenceModel;

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

        public static UpdateAccountExceptionModel ExpectedUpdateAccountExceptionModel
        {
            get
            {
                expectedUpdateAccountExceptionModel = expectedUpdateAccountExceptionModel ?? GenerateSampleUpdateAccountExceptionModel();
                return expectedUpdateAccountExceptionModel;
            }
        }

        public static UpdatePlaceEvidenceModel ExpectedUpdatePlaceEvidenceModel
        {
            get
            {
                expectedUpdatePlaceEvidenceModel = expectedUpdatePlaceEvidenceModel ?? GenerateSampleUpdatePlaceEvidenceModel();
                return expectedUpdatePlaceEvidenceModel;
            }
        }

        public static RemoveEvidenceModel ExpectedRemoveFlaggedEvidenceModel
        {
            get
            {
                expectedRemoveFlaggedEvidenceModel = expectedRemoveFlaggedEvidenceModel ?? GenerateSampleFlaggedEvidenceModel();
                return expectedRemoveFlaggedEvidenceModel;
            }
        }

        public static RemoveEvidenceModel ExpectedRetractAffectedEvidenceModel
        {
            get
            {
                expectedRetractAffectedEvidenceModel = expectedRetractAffectedEvidenceModel ?? GenerateSampleRetractAffectedEvidenceModel();
                return expectedRetractAffectedEvidenceModel;
            }
        }

        public static RemoveEvidenceModel ExpectedRestoreAffectedEvidenceModel
        {
            get
            {
                expectedRestoreAffectedEvidenceModel = expectedRestoreAffectedEvidenceModel ?? GenerateSampleRestoreAffectedEvidenceModel();
                return expectedRestoreAffectedEvidenceModel;
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

        private static UpdateAccountExceptionModel GenerateSampleUpdateAccountExceptionModel()
            => new UpdateAccountExceptionModel
            {
                UserId = 8,
                FromCTSCustId = 10515,
                ToCTSCustId = 213419,
                ToUserName = "K8FFAA",
                Comment = "by test"
            };

        private static UpdatePlaceEvidenceModel GenerateSampleUpdatePlaceEvidenceModel()
            => new UpdatePlaceEvidenceModel
            {
                CTSCustId = 213419,
                SubscriberId = 2,
                EvidenceId = 59,
                Comment = "By Test",
                UserId = 8
            };

        private static RemoveEvidenceModel GenerateSampleFlaggedEvidenceModel()
            => new RemoveEvidenceModel
            {
                CTSCustId = 501755,
                EvidenceId = 1,
                Comment = "By Test",
                UserId = 8
            };

        private static RemoveEvidenceModel GenerateSampleRetractAffectedEvidenceModel()
            => new RemoveEvidenceModel
            {
                CTSCustId = 501755,
                EvidenceId = 60,
                Comment = "By Test",
                UserId = 8
            };

        private static RemoveEvidenceModel GenerateSampleRestoreAffectedEvidenceModel()
            => new RemoveEvidenceModel
            {
                CTSCustId = 501755,
                EvidenceId = 60,
                Comment = "By Test",
                UserId = 8
            };

    }
}