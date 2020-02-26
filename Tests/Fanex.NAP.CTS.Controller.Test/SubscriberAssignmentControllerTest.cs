namespace Fanex.NAP.CTS.Controller.Test
{
    using Fanex.NAP.CTS.Controller;
    using Fanex.NAP.CTS.Controller.Test.DbSetting;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.Subscriber;
    using Fanex.NAP.CTS.Resource.Consts;
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
        
        public SubscriberAssignmentControllerTest()
        {
            ConfigurationManager.AppSettings[Consts.SettingEnvironmentKey] = Consts.SettingEnvironmentLocalValue;
            _runner = new SubscriberAssignmentController();
        }

        [Fact]
        public void CanView_Index()
        {
            // Arrange            
            var controllerContextMock = new FakeControllerContext(IdentityName, _runner);
            _runner.ControllerContext = controllerContextMock.Instance;

            // Act
            var actualResult = _runner.Index() as ViewResult;

            // Assert
            Assert.Equal("", actualResult.ViewName);
        }

        [Theory]
        [InlineData("BCHAI")]
        public void Can_GetUserId(string userName)
        {
            // Act
            var actualResult = _runner.GetUserId(userName) as JsonResult;

            // Assert
            Assert.True(actualResult.Data.ToString() != string.Empty);
        }

        //[Theory]
        //[InlineData(8)]
        //public void Can_GetSubscribers(long userId)
        //{
        //    // Act
        //    var actualResult = _runner.GetSubscribers(userId) as JsonResult;

        //    // Assert
        //    Assert.IsType<List<SubscriberAssignment>>(actualResult.Data);
        //}

        [Fact]
        public void Can_AssignSubscribers()
        {
            // Act
            var actualResult = _runner.AssignSubscribers(ExpectedUpdate.ExpectedSubscriberAssignmentUpdate) as JsonResult;
            var actualDBResult = actualResult.Data as DBResult;

            // Assert
            Assert.True(actualDBResult.ErrorCode == 0);
        }
    }
}