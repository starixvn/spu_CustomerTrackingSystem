namespace Fanex.NAP.CTS.ViewModel.Account
{
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.Account.CustomerInfo;
    using System;
    using System.Collections.Generic;

    public class CustomerGeneralStatusViewModel
    {
        public Model.DTO.Role Role { get; set; }

        public int CustId { get; set; }

        public Int64 CTSCustId { get; set; }

        public int CustSubId { get; set; }

        public string UserName { get; set; }

        public string UserName2 { get; set; }

        public int? CustStatusID { get; set; }

        public string StatusName { get; set; }

        public bool RobotUser { get; set; }

        public bool ProblemAccountWithoutBetLimit { get; set; }

        public bool ProblemAccountWithBetLimit { get; set; }

        public bool BetRestrictedAccount { get; set; }

        public bool BetRestrictedSite { get; set; }

        public bool BetRestrictedCurrency { get; set; }

        public bool BetRestrictedSiteAndCurrency { get; set; }

        public bool SmartPunterAccount { get; set; }

        public bool Affected { get; set; }

        public bool Flagged { get; set; }

        public bool FlaggedWithEvi100 { get; set; }

        public bool Delinquency { get; set; }

        public bool DirtyAccount { get; set; }

        public bool HedgeAccount { get; set; }

        public string DangerousOriginal { get; set; }

        public string DangerousABI { get; set; }

        public string DangerousBasketball { get; set; }

        public string DangerousESports { get; set; }

        public string DangerousOtherSports { get; set; }

        public int Abnormal { get; set; }

        public DateTime NextCategoryScanDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public DateTime LastTicketDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool AutoSuspendSoccer { get; set; }

        public bool AutoSuspendOtherSport { get; set; }

        public bool AutoSuspendESport { get; set; }

        public int TotalAssociationByAccount { get; set; }

        public int TotalAssociationByDevice { get; set; }

        public int TotalAssociationByIp { get; set; }

        public string Evidences { get; set; }

        public string Subscriber { get; set; }

        public int SubscriberId { get; set; }

        public bool IsHostPermission { get; set; }

        public bool IsBettingStatisticPermission { get; set; }

        public bool IsViewIPTransactionPermission { get; set; }

        public int CurrentUserId { get; set; }

        public List<FraudCaseModel> FraudCases { get; set; }

        public CustomerGeneralStatusViewModel BindingGeneralDataFromMainDB(CustomerLastTicketDateModel customerLastTicketDateModel, CustomerInfoByCustIdsModel custDangerousModel)
        {
            if (customerLastTicketDateModel != null)
            {
                this.LastTicketDate = customerLastTicketDateModel.TransDate;
            }

            if (custDangerousModel != null)
            {
                this.DangerousOriginal = (custDangerousModel.Danger1 == 0) ? "None" : custDangerousModel.Danger1.ToString();
                this.DangerousABI = (custDangerousModel.Danger2 == 0) ? "None" : custDangerousModel.Danger2.ToString();
                this.DangerousBasketball = (custDangerousModel.Danger3 == 0) ? "None" : custDangerousModel.Danger3.ToString();
                this.DangerousESports = (custDangerousModel.Danger4 == 0) ? "None" : custDangerousModel.Danger4.ToString();
                this.DangerousOtherSports = (custDangerousModel.Danger5 == 0) ? "None" : custDangerousModel.Danger5.ToString();                
            }
           
            return this;
        }

        public CustomerGeneralStatusViewModel BindingDangerous(CustomerInfoByCustIdsModel custDangerousModel)
        {
            if (custDangerousModel != null)
            {
                this.DangerousOriginal = (custDangerousModel.Danger1 == 0) ? "None" : custDangerousModel.Danger1.ToString();
                this.DangerousABI = (custDangerousModel.Danger2 == 0) ? "None" : custDangerousModel.Danger2.ToString();
                this.DangerousBasketball = (custDangerousModel.Danger3 == 0) ? "None" : custDangerousModel.Danger3.ToString();
                this.DangerousESports = (custDangerousModel.Danger4 == 0) ? "None" : custDangerousModel.Danger4.ToString();
                this.DangerousOtherSports = (custDangerousModel.Danger5 == 0) ? "None" : custDangerousModel.Danger5.ToString();
            }

            return this;
        }

        public CustomerGeneralStatusViewModel BindingGeneralDataFromCTS(CustomerInfoByCTSCustIdModel customerInfoByCTSCustIdModel)
        {
            if (customerInfoByCTSCustIdModel != null)
            {
                this.LastLoginDate = customerInfoByCTSCustIdModel.LastLoginTime;
                this.UserName2 = string.IsNullOrEmpty(customerInfoByCTSCustIdModel.UserName2) ? customerInfoByCTSCustIdModel.UserName : customerInfoByCTSCustIdModel.UserName2;
                this.UserName = string.IsNullOrEmpty(customerInfoByCTSCustIdModel.UserName) ? customerInfoByCTSCustIdModel.UserName2 : customerInfoByCTSCustIdModel.UserName;
                this.CreatedDate = customerInfoByCTSCustIdModel.CreatedDate;
                this.Role = new Model.DTO.Role { Id = customerInfoByCTSCustIdModel.RoleID };
                this.CustSubId = customerInfoByCTSCustIdModel.CustSubID;
                this.CustId = customerInfoByCTSCustIdModel.CustID;
                this.Subscriber = string.IsNullOrEmpty(customerInfoByCTSCustIdModel.SubscriberName) ? "N/A" : customerInfoByCTSCustIdModel.SubscriberName;
                this.SubscriberId = customerInfoByCTSCustIdModel.SubscriberID;
                this.CustStatusID = customerInfoByCTSCustIdModel.CustStatusID;
                this.StatusName =  string.IsNullOrEmpty(customerInfoByCTSCustIdModel.CustStatusName) ? "N/A" : customerInfoByCTSCustIdModel.CustStatusName;                
            }

            return this;
        }
    }
}
