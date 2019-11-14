namespace Fanex.NAP.CTS.Controller.Test.Mocks
{
    using Fanex.NAP.CTS.Controller;
    using Fanex.NAP.CTS.ViewModel.Account.Search;
    using Moq;
    using System.Configuration;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class SearchAccountControllerMock : SearchAccountController
    {
        private Mock<SearchAccountFilter> _filter;

        public SearchAccountControllerMock() : base()
        {
        }

        public void SetPrincipal(string identityName)
        {
            var genericIdentity = new GenericIdentity(identityName);
            var genericPrincipal = new GenericPrincipal(genericIdentity, null);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.User).Returns(genericPrincipal);
            this.ControllerContext = new ControllerContext(context.Object, new RouteData(), this);
        }

        public void SetAppSetting(string key, string value)
        {
            ConfigurationManager.AppSettings[key] = value;
        }

        public void GenerateSearchAccountFilter()
        {
            _filter = new Mock<SearchAccountFilter>();
            _filter.Object.Key = "12b";
            _filter.Object.Subscribers = "-1";
        }

        public ActionResult SearchSubscriberAccount()
        {
            return base.SearchSubscriberAccount(_filter.Object);
        }
    }
}