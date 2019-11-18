namespace Fanex.NAP.CTS.Service.Test.Implement
{
    using Fanex.NAP.CTS.SampleData;
    using Fanex.NAP.CTS.Service.Test.DbSetting;
    using Fanex.NAP.CTS.ViewModel.Account.Evidence;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class EvidenceServiceTest
    {
        private readonly EvidenceService _runner;

        public EvidenceServiceTest()
        {
            _runner = new EvidenceService();
        }

        #region Chains to evidence
        [Fact]
        public void Can_GetChainsToEvidence()
        {
            // Act
            var actualResult = _runner.GetChainsToEvidence(ExpectedFilter.ExpectedChainsToEvidenceFilter);

            // Assert
            Assert.IsType<ChainsToEvidence>(actualResult);
        }

        [Fact]
        public void Can_AddExceptionEvidence()
        {
            // Act
            var actualResult = _runner.AddExceptionEvidence(ExpectedUpdate.ExpectedExceptionEvidenceAdd);

            // Assert
            Assert.True(actualResult.ErrorCode == 0);
        }
        #endregion

        #region Retracted and exception evidence
        [Fact]
        public void Can_GetRetractedAndExceptionEvidence()
        {
            // Act
            var actualResult = _runner.GetRetractedAndExceptionEvidence(ExpectedFilter.ExpectedRetractedAndExceptionEvidenceFilter);

            // Assert
            Assert.IsType<RetractedAndExceptionEvidence>(actualResult);
        }

        #endregion

    }
}