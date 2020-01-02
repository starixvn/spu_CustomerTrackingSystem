using Fanex.NAP.CTS.Model;
using Fanex.NAP.CTS.Model.Account;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.CTS.ViewModel.Account
{
    public class CustomerGeneralStatusViewModel
    {
        public Model.DTO.Role Role { get; set; }

        public int CustId { get; set; }

        public int CTSCustId { get; set; }

        public int CustSubId { get; set; }

        public string UserName { get; set; }

        public string UserName2 { get; set; }

        public string AcountStatus { get; set; }

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

        public int DangerousOriginal { get; set; }

        public int DangerouseABI { get; set; }

        public int DangerouseOtherSports { get; set; }

        public int Abnormal { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public DateTime LastTicketDate { get; set; }

        public bool AutoSuspendSoccer { get; set; }

        public bool AutoSuspendOtherSport { get; set; }

        public bool AutoSuspendESport { get; set; }

        public int TotalAssociationByAccount { get; set; }

        public int TotalAssociationByDevice { get; set; }

        public int TotalAssociationByIp { get; set; }

        public string Evidences { get; set; }

        public List<FraudCaseModel> FraudCases { get; set; }

        public CustomerGeneralStatusViewModel BindingGeneralDataFromMainDB(CustomerGeneralStatusModel customerGeneralStatusModel,
            CustomerLastTicketDateModel customerLastTicketDateModel,
            CustomerEsportAutoSuspendStatusModel customerEsportAutoSuspendStatusModel,
            CustomerRobotUserStatusModel customerRobotUserStatusModel)
        {
            if (customerGeneralStatusModel != null)
            {
                this.AcountStatus = GetAccountStatusName(GetAccountStatus(customerGeneralStatusModel));
                this.DangerousOriginal = customerGeneralStatusModel.Danger;
                this.DangerouseABI = customerGeneralStatusModel.Danger2;
                this.DangerouseOtherSports = customerGeneralStatusModel.Danger3;
                this.CustId = customerGeneralStatusModel.CustId;
                this.UserName = customerGeneralStatusModel.Username;
                this.Abnormal = customerGeneralStatusModel.Abnormal;
                this.AutoSuspendSoccer = customerGeneralStatusModel.Autosuspend;
                this.AutoSuspendOtherSport = customerGeneralStatusModel.Autosuspend2;
                this.ProblemAccountWithBetLimit = customerGeneralStatusModel.ProblemAccountWithBetLimit;
                this.ProblemAccountWithoutBetLimit = customerGeneralStatusModel.ProblemAccountWithoutBetLimit;
                this.BetRestrictedAccount = customerGeneralStatusModel.BetRestrictedAccount;
                this.BetRestrictedCurrency = customerGeneralStatusModel.BetRestrictedCurrency;
                this.BetRestrictedSite = customerGeneralStatusModel.BetRestrictedSite;
                this.SmartPunterAccount = customerGeneralStatusModel.SmartPunter;
            }

            if (customerLastTicketDateModel != null)
            {
                this.LastTicketDate = customerLastTicketDateModel.TransDate;
            }

            if (customerEsportAutoSuspendStatusModel != null)
            {
                this.AutoSuspendESport = customerEsportAutoSuspendStatusModel.AutoSuspendLive || customerEsportAutoSuspendStatusModel.AutoSuspendNonLive;
            }

            if (customerRobotUserStatusModel != null)
            {
                this.RobotUser = customerRobotUserStatusModel.IsRobotUser;
            }

            return this;
        }

        public CustomerGeneralStatusViewModel BindingGeneralDataFromCTS(CustomerInfoByCTSCustIdModel customerInfoByCTSCustIdModel, CustomerFlagInfoModel customerFlagInfoModel, CustomerAssociationByCTSCustIdModel customerAssociationByCTSCustIdModel)
        {
            if (customerInfoByCTSCustIdModel != null)
            {
                this.LastLoginDate = customerInfoByCTSCustIdModel.LastLoginTime;
                this.UserName2 = customerInfoByCTSCustIdModel.Username2;
                this.CreatedDate = customerInfoByCTSCustIdModel.CreatedDate;
                this.Role = new Model.DTO.Role { Id = customerInfoByCTSCustIdModel.RoleId };
                this.CustSubId = customerInfoByCTSCustIdModel.CustSubId;
            }

            if (customerFlagInfoModel != null)
            {
                this.Affected = customerFlagInfoModel.Flagged == 0 && customerAssociationByCTSCustIdModel.TotalAccounts > 0;
                this.Flagged = customerFlagInfoModel.Flagged == 2;
                this.FlaggedWithEvi100 = customerFlagInfoModel.Flagged == 1;
            }

            return this;
        }

        public CustomerGeneralStatusViewModel BindingTotalAssociationFromCTS(CustomerAssociationByCTSCustIdModel customerAssociationByCTSCustIdModel)
        {
            if (customerAssociationByCTSCustIdModel != null)
            {
                this.TotalAssociationByAccount = customerAssociationByCTSCustIdModel.TotalAccounts;
                this.TotalAssociationByDevice = customerAssociationByCTSCustIdModel.TotalDevices;
                this.TotalAssociationByIp = customerAssociationByCTSCustIdModel.TotalIps;
            }

            return this;
        }

        public CustomerGeneralStatusViewModel BindingEvidenceListFromCTS(List<CustomerEvidenceByCTSCustIdModel> customerEvidenceByCTSCustIdModelList)
        {
            if (customerEvidenceByCTSCustIdModelList != null && customerEvidenceByCTSCustIdModelList.Any())
            {
                this.Evidences = string.Join(", ", customerEvidenceByCTSCustIdModelList.Select(x => x.EvidenceCode).ToArray());
            }

            return this;
        }

        public CustomerGeneralStatusViewModel BindingGeneralDataFromFS(CustomerProfileFraudInfoModel customerProfileFraudInfoModel)
        {
            if (customerProfileFraudInfoModel.SuspiciouseCustomer != null)
            {
                this.Delinquency = customerProfileFraudInfoModel.SuspiciouseCustomer.IsDelinquent;
                this.DirtyAccount = customerProfileFraudInfoModel.SuspiciouseCustomer.IsDirty;
                this.HedgeAccount = customerProfileFraudInfoModel.SuspiciouseCustomer.IsHedge;

            }

            if (customerProfileFraudInfoModel.FraudCaseList != null && customerProfileFraudInfoModel.FraudCaseList.Any())
            {
                this.FraudCases = customerProfileFraudInfoModel.FraudCaseList;
            }

            return this;
        }

        public AccountStatus GetAccountStatus(CustomerGeneralStatusModel model)
        {
            if (model.CN88Status == -1)
            {
                return GetAccountStatusForMBC(model);
            }

            return GetAccountStatusForCN88(model);
        }

        public int GetAccountStatusId(string name)
        {
            return (int)Enum.Parse(typeof(AccountStatus), name);
        }

        public string GetAccountStatusName(AccountStatus accountStatus)
        {
            return Enum.GetName(typeof(AccountStatus), accountStatus);
        }

        private AccountStatus GetAccountStatusForMBC(CustomerGeneralStatusModel model)
        {
            if (model.Closed)
            {
                return AccountStatus.Closed;
            }

            if (model.Reject)
            {
                return AccountStatus.Suspended;
            }

            return AccountStatus.Open;
        }

        private AccountStatus GetAccountStatusForCN88(CustomerGeneralStatusModel model)
        {
            switch (model.CN88Status)
            {
                case 1:
                    return AccountStatus.Inactive;
                case 2:
                    return AccountStatus.Suspended;
                case 3:
                    return AccountStatus.ViewOnly;
                default:
                    return AccountStatus.Active;
            }
        }
    }
}
