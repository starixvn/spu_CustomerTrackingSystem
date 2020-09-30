namespace Fanex.NAP.CTS.ViewModel.Account
{
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.API;

    public class AccountProfileViewModel
    {
        public CustomerGeneralStatusModel GeneralStatusInfo { get; set; }

        public string StatusLabel { get; set; }

        public CustomerPoliciesModel PoliciesInfo { get; set; }

        public CustomerRobotUserStatusModel RobotUserStatus { get; set; }

        public CustomerFlagInfoModel FlagedAffectedEvidence { get; set; }

        public CustomerEsportAutoSuspendStatusModel EsportAutoSuspendStatus { get; set; }
    }
}
