namespace Fanex.NAP.CTS.Test.Fake
{
    using Moq;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class FakeControllerContext
    {
        private string _identityName;
        private Controller _controller;
        private ControllerContext _controllerContext;

        public FakeControllerContext(string identityName, Controller controller)
        {
            _identityName = identityName;
            _controller = controller;
        }

        public ControllerContext Instance
        {
            get
            {
                if (_controllerContext == null)
                {
                    var context = new Mock<HttpContextBase>();
                    context.SetupGet(ctx => ctx.User).Returns(GenerateGenericPrincipal());

                    _controllerContext = new ControllerContext(context.Object, new RouteData(), _controller);
                    
                }

                return _controllerContext;
            }
        }

        private GenericPrincipal GenerateGenericPrincipal()
        {
            var genericIdentity = new GenericIdentity(this._identityName);
            var genericPrincipal = new GenericPrincipal(genericIdentity, null);

            return genericPrincipal;
        }
    }
}