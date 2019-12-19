using Fanex.NAP.CTS.Model;
using Fanex.NAP.CTS.Model.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.ViewModel.Account
{
    public class CustomerGeneralStatusViewModel
    {      
        public int CustId { get; set; }

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

        public CustomerGeneralStatusViewModel BindingDataFromMainDB(CustomerGeneralStatusModel model)
        {
            if (model != null)
            {
                this.AcountStatus = Enum.GetName(typeof(AccountStatus), GetAccountStatus(model));
                this.DangerousOriginal = model.Danger;
                this.DangerouseABI = model.Danger2;
                this.DangerouseOtherSports = model.Danger3;
                this.CustId = model.CustId;
                this.UserName = model.Username;
                this.Abnormal = model.Abnormal;
                this.AutoSuspendSoccer = model.Autosuspend;
                this.AutoSuspendOtherSport = model.Autosuspend2;

            }

            return this;
        }

        public CustomerGeneralStatusViewModel BindingDataFromCTS(CustomerInfoByCTSCustIdModel model)
        {
            this.LastLoginDate = model.LastLoginTime;
            this.UserName2 = model.Username2;

            return this;
        }

        public CustomerGeneralStatusViewModel BindingDataFromFS()
        {
            return this;
        }

        private AccountStatus GetAccountStatus(CustomerGeneralStatusModel model)
        {
            if (model.CN88Status == -1)
            {
                return GetAccountStatusForMBC(model);
            }

            return GetAccountStatusForCN88(model);
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
                    return AccountStatus.CN88_Inactive;
                case 2:
                    return AccountStatus.CN88_Suspended;
                case 3:
                    return AccountStatus.CN88_ViewOnly;
                default:
                    return AccountStatus.CN88_Active;
            }
        }
    }
}
