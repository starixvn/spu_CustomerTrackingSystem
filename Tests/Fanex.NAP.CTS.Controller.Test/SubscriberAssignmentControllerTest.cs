namespace Fanex.NAP.CTS.Test
{
    using Fanex.NAP.CTS.Controller;
    using Fanex.NAP.CTS.Controller.Test.DbSetting;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.Subscriber;
    using Fanex.NAP.CTS.SampleData;
    using Fanex.NAP.CTS.Test.Fake;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web.Mvc;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class SubscriberAssignmentControllerTest
    {
        private readonly SubscriberAssignmentController _runner;
        private const string IdentityName = "sa";
        private const string Environment = "Environment";
        private const string EnvironmentLocal = "1";

        public SubscriberAssignmentControllerTest()
        {
            _runner = new SubscriberAssignmentController();
        }

        [Fact]
        public void Index_Test()
        {
            // Arrange
            ConfigurationManager.AppSettings[Environment] = EnvironmentLocal;
            var controllerContextMock = new FakeControllerContext(IdentityName, _runner);
            _runner.ControllerContext = controllerContextMock.Instance;

            // Act
            var actualResult = _runner.Index() as ViewResult;

            // Assert
            Assert.Equal("", actualResult.ViewName);
        }

        [Theory]
        [InlineData("BCHAI")]
        public void HasPermission_GetUserId(string userName)
        {
            // Act
            var actualResult = _runner.GetUserId(userName) as JsonResult;

            // Assert
            Assert.True(actualResult.Data.ToString() != string.Empty);
        }

        [Theory]
        [InlineData(8)]
        public void Can_GetSubscribers_Test(long userId)
        {
            // Act
            var actualResult = _runner.GetSubscribers(userId) as JsonResult;

            // Assert
            Assert.IsType<List<SubscriberAssignment>>(actualResult.Data);
        }

        [Fact]
        public void UpdateSuccess_AssignSubscribers()
        {
            // Act
            var actualResult = _runner.AssignSubscribers(ExpectedUpdate.ExpectedSubscriberAssignmentUpdate) as JsonResult;
            var actualDBResult = actualResult.Data as DBResult;

            // Assert
            Assert.True(actualDBResult.ErrorCode == 0);
        }
    }
}