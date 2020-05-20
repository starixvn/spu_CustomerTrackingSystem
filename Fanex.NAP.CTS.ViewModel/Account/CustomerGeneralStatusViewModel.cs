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

        public Int64 CTSCustId { get; set; }

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

        public string Subscriber { get; set; }

        public int SubscriberId { get; set; }

        public bool IsHostPermission { get; set; }

        public List<FraudCaseModel> FraudCases { get; set; }

        public CustomerGeneralStatusViewModel BindingGeneralDataFromMainDB(
            CustomerLastTicketDateModel customerLastTicketDateModel
           )
        {            

            if (customerLastTicketDateModel != null)
            {
                this.LastTicketDate = customerLastTicketDateModel.TransDate;
            }            

            return this;
        }

        public CustomerGeneralStatusViewModel BindingGeneralDataFromCTS(
            CustomerInfoByCTSCustIdModel customerInfoByCTSCustIdModel            )
        {
            if (customerInfoByCTSCustIdModel != null)
            {
                this.LastLoginDate = customerInfoByCTSCustIdModel.LastLoginTime;
                this.UserName2 = string.IsNullOrEmpty(customerInfoByCTSCustIdModel.Username2) ? customerInfoByCTSCustIdModel.Username : customerInfoByCTSCustIdModel.Username2;
                this.UserName = string.IsNullOrEmpty(customerInfoByCTSCustIdModel.Username) ? customerInfoByCTSCustIdModel.Username2 : customerInfoByCTSCustIdModel.Username;
                this.CreatedDate = customerInfoByCTSCustIdModel.CreatedDate;
                this.Role = new Model.DTO.Role { Id = customerInfoByCTSCustIdModel.RoleId };
                this.CustSubId = customerInfoByCTSCustIdModel.CustSubId;
                this.CustId = customerInfoByCTSCustIdModel.CustId;
                this.Subscriber = customerInfoByCTSCustIdModel.SubscriberName;
                this.SubscriberId = customerInfoByCTSCustIdModel.SubscriberId;
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
                if (model.IsLicensee == 1) // 12bet
                {
                    if (this.Role != null && this.Role.Id == 1) // for member
                    {
                        return AccountStatus.NA;
                    }
                    else
                    {
                        return GetAccountStatusForMBC(model);
                    }
                }

                // mbc
                return GetAccountStatusForMBC(model);
            }

            //cn88
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
            if(model.Enabled)
            {
                if(!model.Closed && !model.Reject)
                {
                    return AccountStatus.Open;
                }

                if(!model.Closed && model.Reject)
                {
                    return AccountStatus.Suspended;
                }

                if(model.Closed)
                {
                    return AccountStatus.Closed;
                }
            }

            return AccountStatus.Disable;
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
