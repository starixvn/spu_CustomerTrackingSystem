namespace Fanex.NAP.CTS.Portal.API
{
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Sentry.RavenClient;
    using Newtonsoft.Json.Serialization;
    using SharpRaven;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
           
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DbSettingProviderManager
                .StartNewSession()
                .WithMySql()
                .WithSqlServer()
                .UseDefaultDbSettingProvider(resourcePath: "~/App_Data/SQL/SPNames.xml")
                .UseDefaultConnectionStringProvider()
                .Run();

            LogManager.SetDefaultLogCategory("PortalAPI")
                   .Use(new List<ILogEngine>
                   {
                         new SentryLogging(new SentryEngineOptions
                         {
                             Dsn = new Dsn(ConfigurationManager.AppSettings["Sentry-Log-Config"])
                         })
                   });

            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
