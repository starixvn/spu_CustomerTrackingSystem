namespace Fanex.NAP.CTS.Site
{
    using Fanex.Logging;
    using Fanex.BetList.Core;
    using Fanex.BetList.Core.RemotingRef;
    using Fanex.Caching;
    using Fanex.Data;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.RemotingRef;
    using ONEbook.UM;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.NETFramework;
    using ONEbook.UM.Transport;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using Fanex.Logging.Sentry.RavenClient;
    using SharpRaven;

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            var umConfig = new UMClientConfiguration().Initialize();

            UMClientManager
               .UseConfig(umConfig)
               .UseCacheService(new UMCacheService(new CacheService()))
               .UseRestClient(new UMRestClient(umConfig))
               .UseHttpContext(new HttpContextHelper());

            LogManager.SetDefaultLogCategory(ConfigurationManager.AppSettings["Sentry-Log-Category"])
                  .Use(new List<ILogEngine>
                  {
                         new SentryLogging(new SentryEngineOptions
                         {
                             Dsn = new Dsn(ConfigurationManager.AppSettings["Sentry-Log-Config"])
                         })
                  });

            var setting = DbSettingProviderManager
                .StartNewSession()
                .Register(true);
            
            setting.Run();

            RefDataTypeContainer
                .ConnectTo(new Uri(ConfigurationManager.AppSettings["Fanex.RemotingRef.Service.Url"]))
                .FromApplication("SPU - Customer Tracking System");

            BetListManager.RegisterRemotingRef(new CacheRef());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            AuthConfig.RegisterAuth(Context);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();

            ex.LogError();
        }
    }
}