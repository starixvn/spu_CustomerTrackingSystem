namespace Fanex.NAP.CTS.Test.Mocks
{
    using Moq;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class ControllerContextMock
    {
        private string _identityName;
        private Controller _controller;

        public ControllerContextMock(string identityName, Controller controller)
        {
            _identityName = identityName;
            _controller = controller;
        }

        public ControllerContext Instance
        {
            get
            {
                var genericIdentity = new GenericIdentity(this._identityName);
                var genericPrincipal = new GenericPrincipal(genericIdentity, null);

                var context = new Mock<HttpContextBase>();
                context.SetupGet(x => x.User).Returns(genericPrincipal);

                var controllerContext = new ControllerContext(context.Object, new RouteData(), _controller);

                return controllerContext;
            }
        }
    }
}