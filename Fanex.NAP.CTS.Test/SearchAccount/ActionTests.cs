namespace Fanex.NAP.CTS.Test.Site.SearchAccount
{
    using Fanex.NAP.CTS.Controller;
    using Moq;    
    using System.Security.Principal;
    using System.Web.Routing;
    using System.Web;
    using System.Web.Mvc;
    using Xunit;
    using System.Configuration;

    public class ActionTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("fail")]
        public void Index_Test(string viewName)
        {
            // Arrange
            var identity = new GenericIdentity("sa");            
            var principal = new GenericPrincipal(identity, null);

            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Returns(true);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.User).Returns(principal);

            var controller = new SearchAccountController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            ConfigurationManager.AppSettings["Environment"] = "1";

            // Act
            var result = controller.Index() as ViewResult;

            // Result
            Assert.Equal(viewName, result.ViewName);
        }
       
    }
}