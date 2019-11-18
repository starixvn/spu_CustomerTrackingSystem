namespace Fanex.NAP.CTS.Test.Fake
{
    using Fanex.NAP.CTS.Resource.Consts;
    using System.IO;
    using System.Reflection;
    using System.Web;
    using System.Web.SessionState;

    public static class FakeHttpContext
    {
        public static void SetCurrentContext()
        {
            var httpRequest = new HttpRequest("", "http://localhost/", "");
            var httpResponce = new HttpResponse(new StringWriter());
            var httpContext = new HttpContext(httpRequest, httpResponce);
            var sessionContainer = new HttpSessionStateContainer("id",
                                       new SessionStateItemCollection(),
                                       new HttpStaticObjectsCollection(),
                                       10,
                                       true,
                                       HttpCookieMode.AutoDetect,
                                       SessionStateMode.InProc,
                                       false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                            BindingFlags.NonPublic | BindingFlags.Instance,
                            null,
                            CallingConventions.Standard,
                            new[] { typeof(HttpSessionStateContainer) },
                            null).Invoke(new object[] { sessionContainer });

            HttpContext.Current = httpContext;
        }

        public static void SetFakeSession()
        {
            HttpContext.Current.Session.Add(Consts.CurrentSubscriberAssignmentsKey, true);
            HttpContext.Current.Session.Add(Consts.IsPermissionCN88Key, true);
        }
    }
}