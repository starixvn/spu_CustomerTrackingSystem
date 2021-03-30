namespace Fanex.NAP.CTS.Model.GraphDB
{
    using System;
    using System.Collections.Generic;

    public class GraphAssociationModel
    {        
        public long CTSCustID { get; set; }

        public int? CustId { get; set; }

        public int CustSubId { get; set; }

        public int SubscriberId { get; set; }

        public string SubscriberName { get; set; }

        public string Username { get; set; }
        
        public string Username2 { get; set; }
        
        public List<int> AssociationType { get; set; }
        
        public string AssociationTypeName { get; set; }

        public DateTime AssociationDateGMT0 { get; set; }

        public DateTime AssociationDate { get; set; }
               
        //GTM-4 for GraphDB
        public DateTime AssociationDateFromGraphDB {
            get => AssociationDateGMT0.AddHours(-4);
        }

        public int SharedDevices { get; set; }
                
        public int SharedIPs { get; set; }
                
        public string StatusName { get; set; }
                
        public int? StatusId { get; set; }
                
        public int AssociationStatus { get; set; }

        public int? SiteID { get; set; }

        public string SiteName { get; set; }

        public string ParentIDs { get; set; }

        public string CategoryIDs { get; set; }

        public string CategoryNames { get; set; }

        public string CustomerClasses { get; set; }
    }
}
