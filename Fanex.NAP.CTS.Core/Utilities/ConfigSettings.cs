namespace Fanex.NAP.CTS.Core.Utilities
{
    using System.Configuration;

    public static class ConfigSettings
    {
        public static string GetConnectionStringByName(string name)
        {
            string returnValue = null;

            var settings = ConfigurationManager.ConnectionStrings[name];

            if (settings != null)
            {
                returnValue = settings.ConnectionString;
            }

            return returnValue;
        }

        public static string GetConfigStringByName(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }

        public static string RabbitMQ_Host()
        {
            return ConfigurationManager.AppSettings["RabbitMQ-Host"];
        }

        public static string RabbitMQ_VirtualHost()
        {
            return ConfigurationManager.AppSettings["RabbitMQ-VirtualHost"];
        }

        public static string RabbitMQ_PrefetchCount()
        {
            return ConfigurationManager.AppSettings["RabbitMQ-PrefetchCount"];
        }

        public static string RabbitMQ_Username()
        {
            return ConfigurationManager.AppSettings["RabbitMQ-Username"];
        }

        public static string RabbitMQ_Password()
        {
            return ConfigurationManager.AppSettings["RabbitMQ-Password"];
        }

        public static string RabbitMQ_Port()
        {
            return ConfigurationManager.AppSettings["RabbitMQ-Port"];
        }

        public static string RabbitMQ_Exchange()
        {
            return ConfigurationManager.AppSettings["RabbitMQ-Exchange"];
        }

        public static string GetDirectAssociation => GetConfigStringByName("Graph_GetDirectAssociation");

        public static string GetAssociationException => GetConfigStringByName("Graph_GetAssociationException");

        public static string GetDirectAssociationByDevice => GetConfigStringByName("Graph_GetDirectAssociationByDevice");

        public static string GetDirectAssociationByIP => GetConfigStringByName("Graph_GetDirectAssociationByIP");

        public static string GetDirectAssociationByManual => GetConfigStringByName("Graph_GetDirectAssociationByManual");

        public static string GetInDirectAssociation => GetConfigStringByName("Graph_GetInDirectAssociation");

        public static string GetShortestAssociation => GetConfigStringByName("Graph_GetShortestPath");

        public static string GetDirectAssociationMultiCust => GetConfigStringByName("Graph_GetDirectAssociationMultiCust");

        public static string GetStatisticAssociation => GetConfigStringByName("Graph_GetStatisticAssociation");

        public static string Matomo_SiteId => GetConfigStringByName("Matomo-SiteId");

        public static string StoredProcedurePath => GetConfigStringByName("StoredProcedurePath");
    }
}