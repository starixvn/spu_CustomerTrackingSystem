using Fanex.NAP.CTS.Core.Helpers.Xml;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Fanex.NAP.CTS.Model.GraphDB
{
    public class GraphAssociationModel
    {        
        public long CTSCustID { get; set; }

        public int? CustId { get; set; }

        public int CustSubId { get; set; }

        public int SubscriberId { get; set; }
        
        public string Username { get; set; }
        
        public string Username2 { get; set; }
        
        public int AssociationType { get; set; }
        
        public string AssociationTypeName
        {
            get;
            set;
        }

        public DateTime AssociationDateGMT0 { get; set; }

        public DateTime AssociationDate
        {
            get => AssociationDateGMT0.AddHours(-4);
        }

        public int SharedDevices { get; set; }
                
        public int SharedIPs { get; set; }
                
        public string StatusName { get; set; }
                
        public int? StatusId { get; set; }
                
        public int AssociationStatus { get; set; }

        public string GetAssociationTypeName(AssociationType associationType)
        {
            return Enum.GetName(typeof(AssociationType), associationType);
        }

        public string GetAssociationTypeName(int associationType)
        {
            string x = Enum.GetName(typeof(AssociationType), associationType);

            if (!string.IsNullOrEmpty(x) && x.Equals("BettingPattern"))
            {
                return "Betting Pattern";
            }

            return x;
        }

        public int? SiteID { get; set; }

        public string SiteName { get; set; }
    }
}
