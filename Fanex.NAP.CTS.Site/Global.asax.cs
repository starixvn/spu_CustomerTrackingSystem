namespace Fanex.NAP.CTS.Site
{
    using Fanex.BetList.Core;
    using Fanex.BetList.Core.RemotingRef;
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.RemotingRef;
    using ONEbook.UM.Filters;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            DbSettingProviderManager
                .StartNewSession()
                .WithSqlServer()
                .WithMySql()
                .UseDefaultDbSettingProvider(ConfigSettings.StoredProcedurePath)
                .UseDefaultConnectionStringProvider()
                .Run();

            LogManager.SetDefaultLogCategory(ConfigurationManager.AppSettings["Sentry-Log-Category"])
                  .Use(new List<ILogEngine>
                  {
                         new SentryLogging(new SentryEngineOptions
                         {
                             Dsn = new Dsn(ConfigurationManager.AppSettings["Sentry-Log-Config"])
                         })
                  });

            RefDataTypeContainer
                .ConnectTo(new Uri(ConfigurationManager.AppSettings["Fanex.RemotingRef.Service.Url"]))
                .FromApplication("SPU - Customer Tracking System");

            BetListManager.RegisterRemotingRef(new CacheRef());

            AreaRegistration.RegisterAllAreas();
            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalFilters.Filters.Add(new CheckUMFilter());
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            AuthConfig.RegisterAuth(Context);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();

            var logMessage = new LogMessage(ex.Message)
              .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
              .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

            LogService.Error(logMessage);
        }
    }
}