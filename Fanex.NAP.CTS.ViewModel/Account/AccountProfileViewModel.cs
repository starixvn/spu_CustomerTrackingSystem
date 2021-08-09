namespace Fanex.NAP.CTS.ViewModel.Account
{
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.API;

    public class AccountProfileViewModel
    {
        public CustomerGeneralInfoModel GeneralInfo { get; set; }

        public CustomerPoliciesModel PoliciesInfo { get; set; }

        public CustomerRobotUserStatusModel RobotUserStatus { get; set; }

        public CustomerFlagInfoModel FlagedAffectedEvidence { get; set; }

        public CustomerEsportAutoSuspendStatusModel EsportAutoSuspendStatus { get; set; }
    }
}
