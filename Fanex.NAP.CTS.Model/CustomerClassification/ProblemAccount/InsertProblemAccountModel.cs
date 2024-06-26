namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    using Fanex.NAP.CTS.Model.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.CTS.Resource.Consts;
    using System;
    using System.Configuration;

    public class InsertProblemAccountModel
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
            this.CategoryGroup = rescanPADangerous.CategoryID;
            this.IsLicensee = rescanPADangerous.IsLicensee;
            this.IsFromAI = true;
            this.IsDirectPA = null;
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
            this.CategoryGroup = ctsCust.CategoryGroup;
            this.IsLicensee = ctsCust.IsLicensee;
            this.IsFromAI = false;
            this.IsDirectPA = null;
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
            this.CategoryGroup = tWInitialSmartCust.CategoryGroup;
            this.IsLicensee = tWInitialSmartCust.IsLicensee;
            this.IsFromAI = false;
            this.IsDirectPA = null;
            this.IsFromCTS = false;
            this.IsFromTVS = false;
            this.IsFromTW = true;
            this.CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["StarixITUserId"]);
            this.Remark = string.Format(Consts.TWInitialSmartRemark, tWInitialSmartCust.DetectedDate.ToString(Consts.FormatDateToString_MMMddyyyy), tWInitialSmartCust.Probability);           
        }

        public InsertProblemAccountModel(RescanTWInitialSmartPAModel tWInitialSmartCust)
        {
            this.CustID = tWInitialSmartCust.CustID;
            this.CTSCustID = tWInitialSmartCust.CTSCustID;
            this.RoleID = tWInitialSmartCust.RoleID;
            this.SubscriberID = tWInitialSmartCust.SubscriberID;
            this.CategoryID = tWInitialSmartCust.CategoryID;
            this.CategoryGroup = tWInitialSmartCust.CategoryGroup;
            this.IsLicensee = tWInitialSmartCust.IsLicensee;
            this.IsFromAI = false;
            this.IsDirectPA = null;
            this.IsFromCTS = false;
            this.IsFromTVS = false;
            this.IsFromTW = true;
            this.CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["StarixITUserId"]);
            this.Remark = $"Detection Date: {tWInitialSmartCust.DetectedDate.ToString(Consts.FormatDateToString_MMMddyyyy)} - Probability: {tWInitialSmartCust.Probability}";
        }

        /* Cust info */
        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public int RoleID { get; set; }

        public int SubscriberID { get; set; }

        public bool? IsLicensee { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string Remark { get; set; }

        public Int64? CreatedBy { get; set; }

        /* Data warehouse, mainDB  info */

        public int? WinlossStatus { get; set; } // LOSING  = 0, KEEPSTATE = 1, WINNING = 2

        public decimal? TurnoverRM { get; set; }

        public decimal? WinlossRM { get; set; }

        public Int64? BetCount { get; set; }

        public int? ActiveDays { get; set; }

        public DateTime? PerformanceTime { get; set; }

        /* Get Mapping Value */

        public int? CategoryID { get; set; }

        public int CategoryGroup { get; set; }

        public int? ReasonID { get; set; }

        public bool? IsDirectPA { get; set; }

        public bool IsFromAI { get; set; }

        public bool IsFromTVS { get; set; }

        public int? TVSRequestID { get; set; }

        public bool? IsParlay { get; set; }

        public int? SportType { get; set; }

        public int IssueTypeID { get; set; }

        public bool IsFromTW { get; set; }

        public bool IsFromCTS { get; set; }

        //True if only removing SpecialCC
        public bool IsDataChanged { get; set; }
    }
}