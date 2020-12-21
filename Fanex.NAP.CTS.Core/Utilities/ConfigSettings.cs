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

        public static string GetAssociations => GetConfigStringByName("Graph_GetAssociations");

        public static string GetAssociationDetail => GetConfigStringByName("Graph_GetAssociationDetail");

        public static string GetAssociatedWithPA => GetConfigStringByName("Graph_GetAssociatedWithPA");

        public static string Matomo_SiteId => GetConfigStringByName("Matomo-SiteId");

        public static string StoredProcedurePath => GetConfigStringByName("StoredProcedurePath");

        public static string AddNewEvidence => GetConfigStringByName("FPS_AddNewEvidence");

        public static int ImportEvidenceChunkSize
        {
            get
            {
                var size = 500;
                var configSize = GetConfigStringByName("Import-Evidence-ChunkSize");

                int.TryParse(configSize, out size);

                return size == 0 ? 500 : size;
            }
        }

        public static int SPUMMailingListId
        {
            get
            {
                var mailingListId = 80;
                var id = GetConfigStringByName("SPU_MailingListId");

                int.TryParse(id, out mailingListId);

                return mailingListId == 0 ? 80 : mailingListId;
            }
        }

        public static string EnvironmentName
        {
            get
            {
                var environment = GetConfigStringByName("Environment");

                switch (environment)
                {
                    case "1":
                        return "LOCAL";
                    case "2":
                        return "UAT";
                    case "3":
                        return "PRO";
                    default:
                        return "LOCAL";
                }
            }
        }

        public static string VoidRequestToTVSURL => GetConfigStringByName("VoidRequestToTVS-URL");
        
        public static int VoidRequestToTVSSubIssueTypeId
        {
            get
            {
                var subIssueTypeId = 0;
                var id = GetConfigStringByName("VoidRequestToTVS-SubIssueTypeId");

                int.TryParse(id, out subIssueTypeId);

                return subIssueTypeId == 0 ? 27 : subIssueTypeId;
            }
        }
            
        public static int VoidRequestToTVSActionId
        {
            get
            {
                var actionId = 0;
                var id = GetConfigStringByName("VoidRequestToTVS-ActionId");

                int.TryParse(id, out actionId);

                return actionId == 0 ? 27 : actionId;
            }
        }

        public static string GetAssociatedGroup => GetConfigStringByName("Graph_GetAssociatedGroup");

        public static int GetAccummulatedChunkSize
        {
            get
            {
                var size = 500;
                var configSize = GetConfigStringByName("Get-Accummulated-ChunkSize");

                int.TryParse(configSize, out size);

                return size == 0 ? 500 : size;
            }
        }

        public static int PushCCToExternalChunkSize
        {
            get
            {
                var size = 150;
                var configSize = GetConfigStringByName("PushCCToExternal-ChunkSize");

                int.TryParse(configSize, out size);

                return size == 0 ? 150 : size;
            }
        }
    }
}