namespace Fanex.NAP.CTS.Model.Account
{
    using System;
    using System.Collections.Generic;

    public class AssociatedAccountModel
    {        
        public long CTSCustID { get; set; }

        public long? CustId { get; set; }

        public int CustSubId { get; set; }

        public int SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public string Username { get; set; }
        
        public string Username2 { get; set; }

        public int? StatusId { get; set; }

        public string StatusName { get; set; }

        public int AssociationStatus { get; set; }

        public DateTime AssociationDate { get; set; }

        public string ParentIDs { get; set; }

        public string CategoryIDs { get; set; }

        public string CategoryNames { get; set; }

        public List<int> AssociationType { get; set; }
        
        public string AssociationTypeName { get; set; }

        public int SharedDevices { get; set; }
                
        public int SharedIPs { get; set; }
  
        public string CustomerClasses { get; set; }

        public bool IsProbation { get; set; }

        public string GroupIDs { get; set; }

        public string OriginGroupIDs { get; set; }      

        public bool IsPA { get; set; }

        public bool IsPotentialPA { get; set; }

        public string ColorTypeIDs { get; set; }
    }
}
