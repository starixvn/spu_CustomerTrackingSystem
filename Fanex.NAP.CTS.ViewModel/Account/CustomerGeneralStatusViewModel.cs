namespace Fanex.NAP.CTS.ViewModel.Account
{
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.Account.CustomerInfo;
    using System;

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

        public bool IsRobotUser { get; set; }

        public int RobotType { get; set; }

        public bool FlaggedEvidence { get; set; }

        public bool AffectedEvidence { get; set; }        

        public string DangerousOriginal { get; set; }

        public string DangerousABI { get; set; }

        public string DangerousBasketball { get; set; }

        public string DangerousESports { get; set; }

        public string DangerousOtherSports { get; set; }

        public DateTime LastLoginDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Subscriber { get; set; }

        public int SubscriberId { get; set; }

        public bool IsHostPermission { get; set; }

        public bool IsBettingStatisticPermission { get; set; }

        public bool IsViewIPTransactionPermission { get; set; }

        public int CurrentUserId { get; set; }

        public bool IsLicensee { get; set; }

        public bool IsInternal { get; set; }

        public CustomerGeneralStatusViewModel BindingDangerous(CustomerInfoByCustIdsModel custDangerousModel)
        {
            if (custDangerousModel != null)
            {
                this.DangerousOriginal = (custDangerousModel.Danger1 == 0) ? "None" : custDangerousModel.Danger1.ToString();
                this.DangerousABI = (custDangerousModel.Danger2 == 0) ? "None" : custDangerousModel.Danger2.ToString();
                this.DangerousBasketball = (custDangerousModel.Danger4 == 0) ? "None" : custDangerousModel.Danger4.ToString();
                this.DangerousESports = (custDangerousModel.Danger5 == 0) ? "None" : custDangerousModel.Danger5.ToString();
                this.DangerousOtherSports = (custDangerousModel.Danger3 == 0) ? "None" : custDangerousModel.Danger3.ToString();
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
                this.StatusName = string.IsNullOrEmpty(customerInfoByCTSCustIdModel.CustStatusName) ? "N/A" : customerInfoByCTSCustIdModel.CustStatusName;
                this.IsLicensee = customerInfoByCTSCustIdModel.IsLicensee;
                this.IsInternal = customerInfoByCTSCustIdModel.IsInternal;
            }

            return this;
        }

        public CustomerGeneralStatusViewModel BindingRobotUser(CustomerRobotUserStatusModel robotModel)
        {
            if (robotModel != null)
            {
                this.IsRobotUser = robotModel.IsRobotUser;
                this.RobotType = robotModel.RobotType.HasValue ? robotModel.RobotType.Value : 0;
            }

            return this;
        }

        public CustomerGeneralStatusViewModel BindingEvidences(CustomerFlagInfoModel evidenceModel)
        {
            if (evidenceModel != null)
            {
                this.FlaggedEvidence = evidenceModel.Flagged;
                this.AffectedEvidence = evidenceModel.Affected;
            }

            return this;
        }
    }
}
