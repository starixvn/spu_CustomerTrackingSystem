namespace Fanex.NAP.CTS.Portal.API
{
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Logging;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.NAP.SPU.Graph.Core;
    using Fanex.NAP.SPU.Graph.Core.Neo4j;
    using Newtonsoft.Json.Serialization;
    using SharpRaven;
    using System.Collections.Generic;
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

            DbSessionFactory.Init()
                .RegisterConfigs("GraphDB")
                .RegisterXmlFile("~/App_Data/Graph", "Script.xml")
                .RegisterDriverFactory(new Neo4jDriverFactory())
                .RegisterCommandBuilder(new Neo4jCommandBuilder("~/App_Data/Graph/scripts"))
                .RegisterCommandExecutor(new Neo4jExecutor())
                .Run();

            DbSettingProviderManager
                .StartNewSession()
                .WithMySql()
                .UseDefaultDbSettingProvider("~/App_Data/SQL")
                .UseDefaultConnectionStringProvider()
                .Run();

            LogManager.SetDefaultLogCategory("PortalAPI")
                   .Use(new List<ILogEngine>
                   {
                         new SentryLogging(new SentryEngineOptions
                         {
                             Dsn = new Dsn("http://49a89a5c0163465895db709864620c65@10.99.99.51:801/103")
                         })
                   });

            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
