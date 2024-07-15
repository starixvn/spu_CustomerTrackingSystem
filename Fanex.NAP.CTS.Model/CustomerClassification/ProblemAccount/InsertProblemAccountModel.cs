namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    using Fanex.NAP.CTS.Model.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.CTS.Resource.Consts;
    using System;
    using System.Configuration;

    public class InsertProblemAccountModel : CustomerClassificationModel
    {
        public InsertProblemAccountModel()
        {
            IsFromTVS = false;
            IsFromCTS = false;
            IsFromTW = false;
            IsDataChanged = false;
        }

        public InsertProblemAccountModel(RescanPADangerousModel rescanPADangerous)
        {
            this.CustID = rescanPADangerous.CustID;
            this.CTSCustID = rescanPADangerous.CTSCustID;
            this.RoleID = rescanPADangerous.RoleID;
            this.SubscriberID = rescanPADangerous.SubscriberID;
            this.CategoryID = rescanPADangerous.CategoryID;            
            this.IsLicensee = rescanPADangerous.IsLicensee;
            this.IsFromAI = true;
            this.IsMarkedDirectly = null;
            this.IsFromCTS = false;
            this.IsFromTVS = false;
            this.IsFromTW = false;
            this.CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["StarixITUserId"]);
            this.Remark = string.Format(Consts.DangerousScoreRemark, rescanPADangerous.ClassifiedDate.ToString(Consts.FormatDateToString_MMMddyyyy), (rescanPADangerous.ClassifiedScore * 100).ToString(Consts.FormatDecimalToString_2Places));           
        }

        public InsertProblemAccountModel(GetInitialTWGroupBettingModel twGBCust, PotentialPACustInfoModel ctsCust)
        {
            this.CustID = twGBCust.CustID;
            this.CTSCustID = ctsCust.CTSCustID;
            this.RoleID = ctsCust.RoleID;
            this.SubscriberID = ctsCust.SubscriberID;
            this.CategoryID = ctsCust.CategoryID;            
            this.IsLicensee = ctsCust.IsLicensee;
            this.IsFromAI = false;
            this.IsMarkedDirectly = null;
            this.IsFromCTS = false;
            this.IsFromTVS = false;
            this.IsFromTW = true;
            this.CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["StarixITUserId"]);
            this.Remark = string.Format(Consts.TWTWInitialGBRemark, twGBCust.DetectedDate.ToString(Consts.FormatDateToString_MMMddyyyy), twGBCust.GBTicketCount, twGBCust.SportName);            
        }

        public InsertProblemAccountModel(TWInitialSmartPAModel tWInitialSmartCust)
        {
            this.CustID = tWInitialSmartCust.CustID;
            this.CTSCustID = tWInitialSmartCust.CTSCustID;
            this.RoleID = tWInitialSmartCust.RoleID;
            this.SubscriberID = tWInitialSmartCust.SubscriberID;
            this.CategoryID = tWInitialSmartCust.CategoryID;
            this.IsLicensee = tWInitialSmartCust.IsLicensee;
            this.IsFromAI = false;
            this.IsMarkedDirectly = null;
            this.IsFromCTS = false;
            this.IsFromTVS = false;
            this.IsFromTW = true;
            this.CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["StarixITUserId"]);
            this.Remark = string.Format(Consts.TWInitialSmartRemark, tWInitialSmartCust.DetectedDate.ToString(Consts.FormatDateToString_MMMddyyyy), tWInitialSmartCust.Probability);           
        }

        public InsertProblemAccountModel(RescanCustomerClassificationModel data)
        {
            CTSCustID = data.CTSCustID;
            CustID = data.CustID;
            CategoryID = data.CategoryID;
            ReasonID = data.TVSReasonID;
            RoleID = data.RoleID;
            SubscriberID = data.SubscriberID;
            IsLicensee = data.IsLicensee;
            Remark = data.Remark;
            CreatedBy = data.CreatedBy;
            IsMarkedDirectly = data.IsMarkedDirectly;
            IsFromTVS = data.IsFromTVS;
            IsFromTW = data.IsFromTW;
            IsFromCTS = data.IsFromCTS;
            IsFromAI = data.IsFromAI;
            IsFromImperva = data.IsFromImperva;
            LastModifiedDate = data.LastModifiedDate;
        }     

        public long CTSCustID { get; set; }

        public int RoleID { get; set; }

        public int SubscriberID { get; set; }

        public bool? IsLicensee { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string Remark { get; set; }

        public Int64? CreatedBy { get; set; }

        /* Data warehouse, mainDB  info */

        public int? WinlossStatus { get; set; } // LOSING  = 0, KEEPSTATE = 1, WINNING = 2      

        public int? ReasonID { get; set; }

        public bool? IsMarkedDirectly { get; set; }

        public bool IsFromAI { get; set; }

        public bool IsFromTVS { get; set; }

        public int? TVSRequestID { get; set; }

        public bool? IsParlay { get; set; }

        public int? SportType { get; set; }

        public int IssueTypeID { get; set; }

        public bool IsFromTW { get; set; }

        public bool IsFromCTS { get; set; }

        public bool IsFromImperva { get; set; }

        //True if only removing SpecialCC
        public bool IsDataChanged { get; set; }
    }
}