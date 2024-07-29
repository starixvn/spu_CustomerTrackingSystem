namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Resource.Consts;
    using System.Collections.Generic;
    using System.Linq;

    public class CustomerAssociationByAccountDetailViewModel
    {       
        public CustomerAssociationByAccountDetailViewModel(AssociatedAccountModel data)
        {
            this.CustId = data.CustId.HasValue ? data.CustId.Value : 0;
            this.Username = !string.IsNullOrWhiteSpace(data.Username) ? data.Username : data.Username2;
            this.LoginId = !string.IsNullOrWhiteSpace(data.Username2) ? data.Username2 : data.Username;
            this.AssociationDate = data.AssociationDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss);
            this.AssociationType = data.AssociationType;
            this.AssociationTypeName = StandardizeAssociationTypeName(data.AssociationTypeName);
            this.Status = data.StatusName == "NA" ? "N/A" : data.StatusName;
            this.StatusId = data.StatusId.HasValue ? data.StatusId.Value : 0;
            this.SubscriberId = data.SubscriberID;
            this.SubscriberName = !string.IsNullOrEmpty(data.SubscriberName) ? data.SubscriberName : "-";
            this.CTSCustId = data.CTSCustID;
            this.IsLinked = data.AssociationStatus == 1;
            this.IsProblemAccount = data.IsPA;
            this.IsPotentialPA = data.IsPotentialPA;         
            this.IsPAProbation = data.IsProbation;
            this.CategoryIds = data.CategoryIDs;
            this.CategoryNames = !string.IsNullOrEmpty(data.CategoryNames) ? data.CategoryNames : "-";
            this.CustomerClasses = data.CustomerClasses != "-1" ? data.CustomerClasses : "-";
            this.GroupIDs = data.GroupIDs;
            this.OriginGroupIDs = data.OriginGroupIDs;
            int.TryParse(data.ColorTypeIDs, out int colorTypeID);
            this.CategoryColorType = (CategoryColorType)colorTypeID;
        }

        public long CTSCustId { get; set; }

        public long CustId { get; set; }

        public int SubscriberId { get; set; }

        public string Username { get; set; }

        public string LoginId { get; set; }

        public bool IsAffectedTag { get; set; }

        public bool IsFlaggedTag { get; set; }

        public string Status { get; set; }

        public int StatusId { get; set; }

        public int SiteId { get; set; }

        public string SiteName { get; set; }

        public string SubscriberName { get; set; }

        public List<int> AssociationType { get; set; }

        public string AssociationTypeName { get; set; }

        public bool IsProblemAccount { get; set; }

        public bool IsPotentialPA { get; set; }

        public string CategoryIds { get; set; }

        public string CategoryNames { get; set; }     

        public string CustomerClasses { get; set; }

        public string AssociationDate { get; set; }

        public bool IsLinked { get; set; }

        public bool IsPAProbation { get; set; }
        
        public string GroupIDs { get; set; }

        public string OriginGroupIDs { get; set; }

        public CategoryColorType CategoryColorType { get; set; }

        private  string StandardizeAssociationTypeName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var spanBettingPattern = $"<span class='association-type-name betting-pattern'>{Consts.AssociationTypeName_BettingPattern}</span>";
                var spanIPAssociation = $"<span class='association-type-name ip-association'>{Consts.AssociationTypeName_IP}</span>";

                name = name.Replace(Consts.AssociationTypeName_BettingPattern, spanBettingPattern);
                name = name.Replace(Consts.AssociationTypeName_IP, spanIPAssociation);
            }

            return name;
        }

        public string CategoryColorDesc
        {
            get
            {
                return EnumExtensions.GetStringSlow(CategoryColorType);
            }
        }
    }
}
